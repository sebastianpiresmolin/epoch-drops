local allowedRealms = {"Menethil", "Gurubashi", "Kezan", "Uldaman"}
local isAllowedRealm = false
local initialized = false

-- =========================================================
-- Simple debug facility (persists in SavedVariables)
-- =========================================================
Epoch_DropsData = Epoch_DropsData or {}
Epoch_DropsData.sessionStarted = Epoch_DropsData.sessionStarted or date("%Y-%m-%d %H:%M:%S")
if Epoch_DropsData.debug == nil then Epoch_DropsData.debug = false end
local DEBUG = Epoch_DropsData.debug

local function setDebug(v)
    DEBUG = not not v
    Epoch_DropsData.debug = DEBUG
    print(string.format("|cff9999ff[Epoch_Drops]|r Debug %s.", DEBUG and "|cff00ff00ON|r" or "|cffff0000OFF|r"))
end

local function dbg(fmt, ...)
    if not DEBUG then return end
    if select("#", ...) > 0 then
        print("|cff9999ff[ED]|r " .. string.format(fmt, ...))
    else
        print("|cff9999ff[ED]|r " .. tostring(fmt))
    end
end

-- =========================================================
-- Tooltip scanner (for storing item tooltip lines)
-- =========================================================
local scanner = CreateFrame("GameTooltip", "EpochTooltipScanner", nil, "GameTooltipTemplate")
scanner:SetOwner(WorldFrame, "ANCHOR_NONE")

local function GetTooltipLines(link)
    scanner:ClearLines()
    scanner:SetHyperlink(link)
    local lines = {}
    for i = 1, scanner:NumLines() do
        local leftLine = _G["EpochTooltipScannerTextLeft" .. i]
        if leftLine then
            local text = leftLine:GetText()
            if text and text ~= "" then
                table.insert(lines, text)
            end
        end
    end
    return lines
end

local function escapeString(s)
    s = s:gsub("\\", "\\\\")
    s = s:gsub("\"", "\\\"")
    s = s:gsub("\n", "\\n")
    s = s:gsub("\r", "\\r")
    return "\"" .. s .. "\""
end

local function cleanIconName(icon)
    if not icon or icon == "" then
        return ""
    end
    local iconName = icon:match("Interface[/\\][Ii]cons[/\\](.+)") or icon
    return iconName:lower()
end

local function toJSON(value)
    if type(value) == "string" then
        return escapeString(value)
    elseif type(value) == "number" or type(value) == "boolean" then
        return tostring(value)
    elseif type(value) == "table" then
        local isArray = true
        local maxIndex = 0
        for k, _ in pairs(value) do
            if type(k) ~= "number" then isArray = false break end
            if k > maxIndex then maxIndex = k end
        end

        local result = {}
        if isArray and maxIndex > 0 then
            for i = 1, maxIndex do
                table.insert(result, toJSON(value[i]))
            end
            return "[" .. table.concat(result, ",") .. "]"
        else
            for k, v in pairs(value) do
                local key = escapeString(tostring(k))
                table.insert(result, key .. ":" .. toJSON(v))
            end
            return "{" .. table.concat(result, ",") .. "}"
        end
    else
        return "null"
    end
end

local function SaveAsJson()
    if not Epoch_DropsData then return end

    local jsonArray = {}

    for k, v in pairs(Epoch_DropsData) do
        if k ~= "sessionStarted" and k ~= "debug" then
            local entry = v
            entry.name = k

            -- Convert drops table (id->obj) to array for export
            if entry.drops and type(entry.drops) == "table" then
                local dropsArray = {}
                for _, drop in pairs(entry.drops) do
                    table.insert(dropsArray, drop)
                end
                entry.drops = dropsArray
            end

            table.insert(jsonArray, entry)
        end
    end

    local ok, json = pcall(toJSON, jsonArray)
    if ok then
        Epoch_DropsJSON = json
        dbg("Saved %d entries to Epoch_DropsJSON.", #jsonArray)
    else
        print("|cffff0000[Epoch_Drops] JSON encode failed.|r")
    end
end

-- Position helper (Wrath 3.3.5 API)
local function GetPlayerPos()
    SetMapToCurrentZone()
    local zoneName = GetRealZoneText() or GetZoneText() or "UnknownZone"
    local subZone = GetSubZoneText() or ""
    local x, y = GetPlayerMapPosition("player")
    x = math.floor((x or 0) * 10000) / 100
    y = math.floor((y or 0) * 10000) / 100
    return zoneName, subZone, x, y
end

-- Wrath 3.3.5 compatibility shim
local function GetCLEUArgs(...)
    if CombatLogGetCurrentEventInfo then
        return CombatLogGetCurrentEventInfo()
    else
        return ...
    end
end

-- Fishing detection that won't error if IsFishingLoot() is missing
local function IsFishingLootSafe()
    if IsFishingLoot then
        local ok, result = pcall(IsFishingLoot)
        if ok then return result end
    end
    return false
end

-- =========================================================
-- Event handling
-- =========================================================
local f = CreateFrame("Frame")
f:RegisterEvent("ADDON_LOADED")
f:RegisterEvent("LOOT_OPENED")
f:RegisterEvent("COMBAT_LOG_EVENT_UNFILTERED")

f:SetScript("OnEvent", function(self, event, ...)
    if event == "ADDON_LOADED" then
        if initialized then return end
        initialized = true

        local currentRealm = GetRealmName()
        isAllowedRealm = false
        for _, realm in ipairs(allowedRealms) do
            if currentRealm == realm then
                isAllowedRealm = true
                break
            end
        end

        if not isAllowedRealm then
            print("|cffff0000[Epoch_Drops] Not on allowed realm (" .. (currentRealm or "?") .. "), addon disabled.|r")
        else
            print("|cff00ff00[Epoch_Drops]|r loaded on |cff00ff00" .. (currentRealm or "?") .. "|r")
            dbg("Session started: %s", Epoch_DropsData.sessionStarted)
        end

    elseif event == "LOOT_OPENED" then
        if not isAllowedRealm then return end

        -- ---- FISHING BRANCH ----
        if IsFishingLootSafe() then
            local zoneName, subZone, x, y = GetPlayerPos()
            local key = "Fishing - " .. zoneName
            Epoch_DropsData[key] = Epoch_DropsData[key] or {
                type     = "fishing",
                kills    = 0,  -- treat each catch as a 'kill'
                drops    = {},
                lastSeen = date("%Y-%m-%d %H:%M:%S"),
                location = { zone = zoneName, subZone = subZone, x = x, y = y }
            }
            Epoch_DropsData[key].kills = (Epoch_DropsData[key].kills or 0) + 1
            Epoch_DropsData[key].lastSeen = date("%Y-%m-%d %H:%M:%S")
            Epoch_DropsData[key].location = { zone = zoneName, subZone = subZone, x = x, y = y }

            local drops = Epoch_DropsData[key].drops
            local itemsLogged = 0
            for i = 1, GetNumLootItems() do
                local itemLink = GetLootSlotLink(i)
                local itemName, _, quantity = GetLootSlotInfo(i)
                quantity = quantity or 1

                local name, _, rarity, _, _, itemType, itemSubType, _, equipSlot, icon = GetItemInfo(itemLink or "")
                local itemID = itemLink and tonumber(string.match(itemLink, "item:(%d+):"))
                if itemID then
                    local tooltipLines = GetTooltipLines(itemLink)
                    drops[itemID] = drops[itemID] or {
                        count       = 0,
                        id          = itemID,
                        name        = name or itemName,
                        icon        = cleanIconName(icon),
                        rarity      = rarity,
                        itemType    = itemType,
                        itemSubType = itemSubType,
                        equipSlot   = equipSlot,
                        tooltip     = tooltipLines
                    }
                    drops[itemID].count = drops[itemID].count + quantity
                    itemsLogged = itemsLogged + 1
                    dbg("Fishing drop: +%d x %s (ID %d) -> %s", quantity, drops[itemID].name or "?", itemID, key)
                end
            end

            dbg("Fishing catch recorded in %s at (%.2f, %.2f). Items: %d", key, x or 0, y or 0, itemsLogged)
            SaveAsJson()
            return  -- do not let fishing fall through to 'untracked mob'
        end
        -- ---- END FISHING BRANCH ----

        -- ===== Existing LOOT_OPENED mob flow remains unchanged =====
        local mobName
        if UnitIsDead("target") and UnitCanAttack("player", "target") then
            mobName = UnitName("target") or "Unknown"
        else
            if (MerchantFrame and MerchantFrame:IsShown()) or (MailFrame and MailFrame:IsShown()) or (TradeFrame and TradeFrame:IsShown()) then
                return
            end
            local zone = GetRealZoneText() or GetZoneText() or "UnknownZone"
            local subZone = GetSubZoneText() or ""
            mobName = "[Untracked Mob in " .. zone .. (subZone ~= "" and (":" .. subZone) or "") .. "]"
        end

        local zoneName, subZone, x, y = GetPlayerPos()

        Epoch_DropsData[mobName] = Epoch_DropsData[mobName] or {
            kills = 0,
            drops = {},
            lastSeen = date("%Y-%m-%d %H:%M:%S"),
            location = { zone = zoneName, subZone = subZone, x = x, y = y }
        }
        Epoch_DropsData[mobName].kills = Epoch_DropsData[mobName].kills + 1
        Epoch_DropsData[mobName].lastSeen = date("%Y-%m-%d %H:%M:%S")
        Epoch_DropsData[mobName].location = { zone = zoneName, subZone = subZone, x = x, y = y }
        dbg("Mob loot opened, counted kill for '%s' at (%.2f, %.2f).", mobName, x or 0, y or 0)

        for i = 1, GetNumLootItems() do
            local itemLink = GetLootSlotLink(i)
            local itemName, itemIcon, quantity = GetLootSlotInfo(i)
            quantity = quantity or 1
            local name, _, rarity, _, _, itemType, itemSubType, _, equipSlot, icon = GetItemInfo(itemLink or "")
            local itemID = itemLink and tonumber(string.match(itemLink, "item:(%d+):"))

            if itemID then
                local tooltipLines = GetTooltipLines(itemLink)
                local drops = Epoch_DropsData[mobName].drops

                drops[itemID] = drops[itemID] or {
                    count       = 0,
                    id          = itemID,
                    name        = name or itemName,
                    icon        = cleanIconName(icon or itemIcon),
                    rarity      = rarity,
                    itemType    = itemType,
                    itemSubType = itemSubType,
                    equipSlot   = equipSlot,
                    tooltip     = tooltipLines
                }
                drops[itemID].count = drops[itemID].count + quantity
                dbg("Mob drop: +%d x %s (ID %d) -> %s", quantity, drops[itemID].name or "?", itemID, mobName)
            end
        end

        SaveAsJson()

    elseif event == "COMBAT_LOG_EVENT_UNFILTERED" then
        if not isAllowedRealm then return end
        local _, subevent, _, _, _, _, _, _, destName = GetCLEUArgs(...)
        if subevent == "UNIT_DIED" and UnitExists("target") and UnitIsDead("target") then
            local mobName = UnitName("target")
            if mobName == destName then
                Epoch_DropsData[mobName] = Epoch_DropsData[mobName] or { kills = 0, drops = {} }
                Epoch_DropsData[mobName].kills = Epoch_DropsData[mobName].kills + 1
                dbg("Kill logged from CLEU for '%s'. Total kills: %d", mobName, Epoch_DropsData[mobName].kills)
            end
        end
    end
end)

-- =========================================================
-- Quest reward capture (unchanged, with debug prints)
-- =========================================================
hooksecurefunc("GetQuestReward", function(choiceIndex)
    if not isAllowedRealm then return end

    local npcName = UnitName("target") or "Unknown"
    local questTitle = GetTitleText() or "Unknown Quest"
    local xpReward = GetRewardXP() or 0
    local moneyReward = GetRewardMoney() or 0
    local questKey = questTitle

    local zoneName, subZone, x, y = GetPlayerPos()

    Epoch_DropsData[questKey] = {
        type = "quest",
        name = questTitle,
        giver = npcName,
        lastSeen = date("%Y-%m-%d %H:%M:%S"),
        location = { zone = zoneName, subZone = subZone, x = x, y = y },
        quest = {
            name = questTitle,
            giver = npcName,
            xp = xpReward,
            money = moneyReward,
            reputation = {}
        },
        drops = {}
    }

    for i = 1, GetNumFactions() do
        local name, _, _, repValue, _, _, _, _, isHeader = GetFactionInfo(i)
        if name and not isHeader then
            Epoch_DropsData[questKey].quest.reputation[name] = repValue
        end
    end

    local drops = Epoch_DropsData[questKey].drops
    for i = 1, GetNumQuestChoices() do
        local itemLink = GetQuestItemLink("choice", i)
        local itemID = itemLink and tonumber(itemLink:match("item:(%d+)"))
        if itemID then
            local name, icon, quantity = GetQuestItemInfo("choice", i)
            local _, _, rarity, _, _, itemType, itemSubType, _, equipSlot, fullIcon = GetItemInfo(itemLink or "")
            local tooltipLines = GetTooltipLines(itemLink)
            drops[itemID] = drops[itemID] or {
                count       = 0,
                id          = itemID,
                name        = name,
                icon        = cleanIconName(fullIcon or icon),
                rarity      = rarity,
                itemType    = itemType,
                itemSubType = itemSubType,
                equipSlot   = equipSlot,
                tooltip     = tooltipLines
            }
            drops[itemID].count = drops[itemID].count + (quantity or 1)
            dbg("Quest choice drop: +%d x %s (ID %d) for quest '%s'", quantity or 1, name or "?", itemID, questTitle)
        end
    end

    for i = 1, GetNumQuestRewards() do
        local itemLink = GetQuestItemLink("reward", i)
        local itemID = itemLink and tonumber(itemLink:match("item:(%d+)"))
        if itemID then
            local name, icon, quantity = GetQuestItemInfo("reward", i)
            local _, _, rarity, _, _, itemType, itemSubType, _, equipSlot, fullIcon = GetItemInfo(itemLink or "")
            local tooltipLines = GetTooltipLines(itemLink)
            drops[itemID] = drops[itemID] or {
                count       = 0,
                id          = itemID,
                name        = name,
                icon        = cleanIconName(fullIcon or icon),
                rarity      = rarity,
                itemType    = itemType,
                itemSubType = itemSubType,
                equipSlot   = equipSlot,
                tooltip     = tooltipLines
            }
            drops[itemID].count = drops[itemID].count + (quantity or 1)
            dbg("Quest reward drop: +%d x %s (ID %d) for quest '%s'", quantity or 1, name or "?", itemID, questTitle)
        end
    end

    SaveAsJson()
end)

-- Save on /reload or logout
local saver = CreateFrame("Frame")
saver:RegisterEvent("PLAYER_LOGOUT")
saver:SetScript("OnEvent", SaveAsJson)

-- =========================================================
-- Slash command: /ed (debug only)
-- =========================================================
SLASH_EPOCHDROPS1 = "/ed"
SlashCmdList["EPOCHDROPS"] = function(msg)
    msg = (msg or ""):lower():match("^%s*(.-)%s*$")
    if msg == "on" or msg == "debug on" or msg == "debug" then
        setDebug(true)
    elseif msg == "off" or msg == "debug off" then
        setDebug(false)
    else
        print("|cff9999ff[Epoch_Drops]|r Commands: /ed on, /ed off")
    end
end
