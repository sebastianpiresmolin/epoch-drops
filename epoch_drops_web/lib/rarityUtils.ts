export function getRarityColor(rarity?: number): string {
    switch (rarity) {
        case 0: return "text-gray-400";   // Poor
        case 1: return "text-white";      // Common
        case 2: return "text-green-500";  // Uncommon
        case 3: return "text-blue-500";   // Rare
        case 4: return "text-purple-500"; // Epic
        case 5: return "text-orange-500"; // Legendary
        default: return "text-white";     // Fallback
    }
}
