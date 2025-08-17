local allowedRealms = {"Menethil", "Gurubashi", "Kezan", "Uldaman"}
local isAllowedRealm = false

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

Epoch_DropsData = Epoch_DropsData or {}
Epoch_DropsData.sessionStarted = Epoch_DropsData.sessionStarted or date("%Y-%m-%d %H:%M:%S")

local function SaveAsJson()
    if not Epoch_DropsData then return end

    local jsonArray = {}

    for k, v in pairs(Epoch_DropsData) do
        if k ~= "sessionStarted" then
            local entry = v
            entry.name = k

            -- Convert drops table to array
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
    end
end


-- Create main event frame
local f = CreateFrame("Frame")
f:RegisterEvent("ADDON_LOADED")
f:RegisterEvent("LOOT_OPENED")
f:RegisterEvent("COMBAT_LOG_EVENT_UNFILTERED")

f:SetScript("OnEvent", function(self, event, ...)
    if event == "ADDON_LOADED" then
        local addonName = ...
        if addonName == "Epoch_Drops" then
            local currentRealm = GetRealmName()
            isAllowedRealm = false
            for _, realm in ipairs(allowedRealms) do
                if currentRealm == realm then
                    isAllowedRealm = true
                    break
                end
            end

            if not isAllowedRealm then
                print("|cffff0000[Epoch_Drops] Not on allowed realm (" .. currentRealm .. "), addon disabled.|r")
            else
                print("|cff00ff00[Epoch_Drops loaded on realm: " .. currentRealm .. "]|r")
            end
        end

    elseif event == "LOOT_OPENED" then
        if not isAllowedRealm then return end

        local mobName

        if UnitIsDead("target") and UnitCanAttack("player", "target") then
            mobName = UnitName("target") or "Unknown"
        else
            if MerchantFrame:IsShown() or MailFrame:IsShown() or (TradeFrame and TradeFrame:IsShown()) then
                return
            end

            local zone = GetRealZoneText() or GetZoneText() or "UnknownZone"
            local subZone = GetSubZoneText() or ""
            mobName = "[Untracked Mob in " .. zone .. (subZone ~= "" and (":" .. subZone) or "") .. "]"
        end

        SetMapToCurrentZone()
        local zoneName = GetRealZoneText() or GetZoneText() or "UnknownZone"
        local subZone = GetSubZoneText() or ""
        local x, y = GetPlayerMapPosition("player")
        x = math.floor((x or 0) * 10000) / 100
        y = math.floor((y or 0) * 10000) / 100

        Epoch_DropsData[mobName] = Epoch_DropsData[mobName] or {
            kills = 0,
            drops = {},
            lastSeen = date("%Y-%m-%d %H:%M:%S"),
            location = { zone = zoneName, subZone = subZone, x = x, y = y }
        }
        Epoch_DropsData[mobName].kills = Epoch_DropsData[mobName].kills + 1


        for i = 1, GetNumLootItems() do
            local itemLink = GetLootSlotLink(i)
            local itemName, itemIcon, quantity = GetLootSlotInfo(i)
            quantity = quantity or 1
            local name, _, rarity, _, _, itemType, itemSubType, _, equipSlot, icon = GetItemInfo(itemLink or "")
            local itemID = itemLink and tonumber(string.match(itemLink, "item:(%d+):"))

            if itemID then
                local tooltipLines = GetTooltipLines(itemLink)
                local drops = Epoch_DropsData[mobName].drops
                local iconName = icon and icon:match("Interface\\Icons\\(.+)") or ""

                drops[itemID] = drops[itemID] or {
                    count = 0,
                    id = itemID,
                    name = name or itemName,
                    icon = iconName:lower(),
                    rarity = rarity,
                    itemType = itemType,
                    itemSubType = itemSubType,
                    equipSlot = equipSlot,
                    tooltip = tooltipLines
                }
                drops[itemID].count = drops[itemID].count + quantity
            end
        end

        SaveAsJson()

    elseif event == "COMBAT_LOG_EVENT_UNFILTERED" then
        if not isAllowedRealm then return end
        local _, subevent, _, _, _, _, _, _, destName = CombatLogGetCurrentEventInfo()
        if subevent == "UNIT_DIED" and UnitExists("target") and UnitIsDead("target") then
            local mobName = UnitName("target")
            if mobName == destName then
                Epoch_DropsData[mobName] = Epoch_DropsData[mobName] or { kills = 0, drops = {} }
                Epoch_DropsData[mobName].kills = Epoch_DropsData[mobName].kills + 1
            end
        end
    end
end)

hooksecurefunc("GetQuestReward", function(choiceIndex)
    if not isAllowedRealm then return end

    local npcName = UnitName("target") or "Unknown"
    local questTitle = GetTitleText() or "Unknown Quest"
    local xpReward = GetRewardXP() or 0
    local moneyReward = GetRewardMoney() or 0
    local questKey = questTitle

    SetMapToCurrentZone()
    local zoneName = GetRealZoneText() or GetZoneText() or "UnknownZone"
    local subZone = GetSubZoneText() or ""
    local x, y = GetPlayerMapPosition("player")
    x = math.floor((x or 0) * 10000) / 100
    y = math.floor((y or 0) * 10000) / 100

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
            local tooltipLines = GetTooltipLines(itemLink)
            drops[itemID] = drops[itemID] or {
                count = 0,
                id = itemID,
                name = name,
                icon = icon,
                tooltip = tooltipLines
            }
            drops[itemID].count = drops[itemID].count + (quantity or 1)
        end
    end

    for i = 1, GetNumQuestRewards() do
        local itemLink = GetQuestItemLink("reward", i)
        local itemID = itemLink and tonumber(itemLink:match("item:(%d+)"))
        if itemID then
            local name, icon, quantity = GetQuestItemInfo("reward", i)
            local tooltipLines = GetTooltipLines(itemLink)
            drops[itemID] = drops[itemID] or {
                count = 0,
                id = itemID,
                name = name,
                icon = icon,
                tooltip = tooltipLines
            }
            drops[itemID].count = drops[itemID].count + (quantity or 1)
        end
    end

    SaveAsJson()
end)

-- Save on /reload or logout
local saver = CreateFrame("Frame")
saver:RegisterEvent("PLAYER_LOGOUT")
saver:SetScript("OnEvent", SaveAsJson)
