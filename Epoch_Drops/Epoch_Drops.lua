local allowedRealm = "ChromieCraft"
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

Epoch_DropsData = Epoch_DropsData or {}
Epoch_DropsData.sessionStarted = Epoch_DropsData.sessionStarted or date("%Y-%m-%d %H:%M:%S")

local f = CreateFrame("Frame")
f:RegisterEvent("ADDON_LOADED")
f:RegisterEvent("LOOT_OPENED")
f:RegisterEvent("COMBAT_LOG_EVENT_UNFILTERED")


f:SetScript("OnEvent", function(self, event, ...)
   if event == "ADDON_LOADED" then
    local addonName = ...
    if addonName == "Epoch_Drops" then
        local currentRealm = GetRealmName()
        isAllowedRealm = currentRealm == allowedRealm

        if not isAllowedRealm then
            print("|cffff0000[Epoch_Drops] Not on allowed realm (" .. currentRealm .. "), addon disabled.|r")
        else
            print("|cff00ff00[Epoch_Drops loaded on realm: " .. currentRealm .. "]|r")
        end
    end


   elseif event == "LOOT_OPENED" then
    if not isAllowedRealm then return end
    print("LOOT_OPENED fired")

    local mobName

    -- Priority 1: Valid mob target
    if UnitIsDead("target") and UnitCanAttack("player", "target") then
        mobName = UnitName("target") or "Unknown"
        print("Looting mob:", mobName)

    -- Priority 2: No target, fallback to zone
    else
        -- Skip if vendor, mailbox, or trade window open
        if MerchantFrame:IsShown() or MailFrame:IsShown() or TradeFrame and TradeFrame:IsShown() then
            print("[Epoch_Drops] Loot skipped: vendor/mail/trade window open.")
            return
        end

        -- Fallback assignment
        local zone = GetRealZoneText() or GetZoneText() or "UnknownZone"
        local subZone = GetSubZoneText() or ""
        mobName = "[Untracked Mob in " .. zone .. (subZone ~= "" and (":" .. subZone) or "") .. "]"
        print("Fallback: Assigned to mob:", mobName)
    end

    -- Get position
    SetMapToCurrentZone()
    local zoneName = GetRealZoneText() or GetZoneText() or "UnknownZone"
    local subZone = GetSubZoneText() or ""
    local x, y = GetPlayerMapPosition("player")
    x = math.floor((x or 0) * 10000) / 100
    y = math.floor((y or 0) * 10000) / 100
    print(string.format("Position at loot: %s, %s (%.2f, %.2f)", zoneName, subZone, x, y))

    -- Init mob entry
    Epoch_DropsData[mobName] = Epoch_DropsData[mobName] or {
        kills = 0,
        drops = {},
        lastSeen = date("%Y-%m-%d %H:%M:%S"),
        location = {
            zone = zoneName,
            subZone = subZone,
            x = x,
            y = y,
        }
    }
    Epoch_DropsData[mobName].kills = Epoch_DropsData[mobName].kills + 1

    print(string.format("[Kill+Loot] %s (total kills: %d)", mobName, Epoch_DropsData[mobName].kills))

    -- Process each item
    for i = 1, GetNumLootItems() do
        local itemLink = GetLootSlotLink(i)
        local itemName, itemIcon, quantity = GetLootSlotInfo(i)
        quantity = quantity or 1

        local name, link, rarity, itemLevel, _, itemType, itemSubType, _, equipSlot, icon = GetItemInfo(itemLink or "")
        local itemID = itemLink and tonumber(string.match(itemLink, "item:(%d+):"))

        if itemID then
            local tooltipLines = GetTooltipLines(itemLink)

            local drops = Epoch_DropsData[mobName].drops
            drops[itemID] = drops[itemID] or {
                count = 0,
                id = itemID,
                name = name or itemName,
                icon = itemIcon or icon,
                rarity = rarity,
                itemType = itemType,
                itemSubType = itemSubType,
                equipSlot = equipSlot,
                tooltip = tooltipLines
            }
            drops[itemID].count = drops[itemID].count + quantity

            print(string.format("  Looted %s x%d (%s - %s)", name or itemName, quantity, itemType or "?", itemSubType or "?"))
        end
    end

    elseif event == "COMBAT_LOG_EVENT_UNFILTERED" then
        if not isAllowedRealm then return end

        local timestamp, subevent, _, _, _, _, _, destGUID, destName = CombatLogGetCurrentEventInfo()

        if subevent == "UNIT_DIED" then
            -- If we're targeting the dying mob
            if UnitExists("target") and UnitIsDead("target") then
                local mobName = UnitName("target")
                if mobName == destName then
                    Epoch_DropsData[mobName] = Epoch_DropsData[mobName] or {
                        kills = 0,
                        drops = {}
                    }
                    Epoch_DropsData[mobName].kills = Epoch_DropsData[mobName].kills + 1
                    print(string.format("[Kill] %s (total kills: %d)", mobName, Epoch_DropsData[mobName].kills))
                end
            end
        end
    end
end)

hooksecurefunc("GetQuestReward", function(choiceIndex)
    print("[Epoch_Drops] GetQuestReward hook triggered")

    if not isAllowedRealm then
        print("Wrong realm")
        return
    end

    local npcName = UnitName("target") or "Unknown"
    print("NPC Name:", npcName)

    local questTitle = GetTitleText() or "Unknown Quest"
    print("Quest Title:", questTitle)

    local xpReward = GetRewardXP() or 0
    print("XP Reward:", xpReward)

    local moneyReward = GetRewardMoney() or 0
    print("Money Reward:", moneyReward)

    local mobName = string.format("[Quest Reward from: %s | %s]", npcName, questTitle)

    -- Get position
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
        location = {
            zone = zoneName,
            subZone = subZone,
            x = x,
            y = y,
        },
        quest = {
            title = questTitle,
            xp = xpReward,
            money = moneyReward,
            rep = {}
        }
    }

    for i = 1, GetNumFactions() do
        local name, _, _, repValue, _, _, _, _, isHeader = GetFactionInfo(i)
        if name and not isHeader then
            Epoch_DropsData[mobName].quest.rep[name] = repValue
        end
    end

    print(string.format("[Quest Reward] %s | XP: %d | Gold: %.2fg", questTitle, xpReward, moneyReward / 10000))

    -- Choice items (you choose one)
for i = 1, GetNumQuestChoices() do
    local itemLink = GetQuestItemLink("choice", i)
    local itemID = itemLink and tonumber(itemLink:match("item:(%d+)"))
    if itemID then
        local name, icon, quantity = GetQuestItemInfo("choice", i)
        local tooltipLines = GetTooltipLines(itemLink)
        local drops = Epoch_DropsData[mobName].drops
        drops[itemID] = drops[itemID] or {
            count = 0,
            id = itemID,
            name = name,
            icon = icon,
            tooltip = tooltipLines
        }
        drops[itemID].count = drops[itemID].count + (quantity or 1)
        print(string.format("  [Quest Choice Item] %s x%d", name, quantity or 1))
    end
end

-- Guaranteed reward items (you always get)
for i = 1, GetNumQuestRewards() do
    local itemLink = GetQuestItemLink("reward", i)
    local itemID = itemLink and tonumber(itemLink:match("item:(%d+)"))
    if itemID then
        local name, icon, quantity = GetQuestItemInfo("reward", i)
        local tooltipLines = GetTooltipLines(itemLink)
        local drops = Epoch_DropsData[mobName].drops
        drops[itemID] = drops[itemID] or {
            count = 0,
            id = itemID,
            name = name,
            icon = icon,
            tooltip = tooltipLines
        }
        drops[itemID].count = drops[itemID].count + (quantity or 1)
        print(string.format("  [Quest Reward Item] %s x%d", name, quantity or 1))
    end
end

end)

