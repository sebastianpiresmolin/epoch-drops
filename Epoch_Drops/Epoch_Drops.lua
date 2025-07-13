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

    if UnitIsDead("target") and UnitCanAttack("player", "target") then
        local mobName = UnitName("target") or "Unknown"
        print("Looting mob:", mobName)

        -- Initialize + increment kills
        Epoch_DropsData[mobName] = Epoch_DropsData[mobName] or {
            kills = 0,
            drops = {}
        }
        Epoch_DropsData[mobName].kills = Epoch_DropsData[mobName].kills + 1
        print(string.format("[Kill+Loot] %s (total kills: %d)", mobName, Epoch_DropsData[mobName].kills))

        for i = 1, GetNumLootItems() do
            local itemLink = GetLootSlotLink(i)
            local itemName, itemIcon, quantity = GetLootSlotInfo(i)
            quantity = quantity or 1

            local name, link, rarity, itemLevel, _, itemType, itemSubType, _, equipSlot, icon = GetItemInfo(itemLink or "")
            local itemID = itemLink and tonumber(string.match(itemLink, "item:(%d+):"))

            if itemID then
                -- 📌 Get tooltip lines for flavor/stats
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
                    tooltip = tooltipLines -- ✅ store tooltips here
                }
                drops[itemID].count = drops[itemID].count + quantity

                print(string.format("  Looted %s x%d (%s - %s)", name or itemName, quantity, itemType or "?", itemSubType or "?"))
            end
        end
    else
        print("Target is not a valid dead mob — loot skipped.")
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
