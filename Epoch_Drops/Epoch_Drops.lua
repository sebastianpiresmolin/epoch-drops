local f = CreateFrame("Frame")
f:RegisterEvent("ADDON_LOADED")
f:RegisterEvent("LOOT_OPENED")
f:RegisterEvent("COMBAT_LOG_EVENT_UNFILTERED")

f:SetScript("OnEvent", function(self, event, ...)
    if event == "ADDON_LOADED" then
        local addonName = ...
        if addonName == "MobDropTracker" then
            print("|cff00ff00[MobDropTracker loaded]|r")
        end
    elseif event == "LOOT_OPENED" then
        -- Loot logic will go here
    elseif event == "COMBAT_LOG_EVENT_UNFILTERED" then
        -- Kill tracking will go here
    end
end)
