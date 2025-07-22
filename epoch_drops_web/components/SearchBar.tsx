'use client';
import { useState, useEffect, useRef } from 'react';
import { useRouter } from "next/navigation";

type SearchResult = {
    type: 'Item' | 'Mob' | 'Quest';
    name: string;
    id: number;
    rarity?: number;
    icon?: string;
};

export default function SearchBar() {
    const [query, setQuery] = useState('');
    const [results, setResults] = useState<SearchResult[]>([]);
    const [showDropdown, setShowDropdown] = useState(false);
    const timeoutRef = useRef<NodeJS.Timeout | null>(null);
    const router = useRouter();

    const handleSelect = (result: SearchResult) => {
        const route = `/${result.type.toLowerCase()}/${result.id}`;
        router.push(route);
    };

    const rarityColors: Record<number, string> = {
        0: "text-gray-400",   // Poor (gray)
        1: "text-white",       // Common
        2: "text-green-500",   // Uncommon
        3: "text-blue-500",    // Rare
        4: "text-purple-500",  // Epic
        5: "text-orange-500",  // Legendary
    };

    const getRarityClass = (rarity: number) => rarityColors[rarity] || "text-white";

    useEffect(() => {
        if (!query.trim()) {
            setShowDropdown(false);
            setResults([]);
            return;
        }

        if (timeoutRef.current) clearTimeout(timeoutRef.current);

        timeoutRef.current = setTimeout(async () => {
            try {
                const res = await fetch(`http://epoch-drops-production.up.railway.app/search?q=${encodeURIComponent(query.trim())}`);
                const data = await res.json();
                setResults(data);
                setShowDropdown(true);
            } catch (err) {
                console.error('Search failed', err);
                setResults([]);
            }
        }, 1000); // debounce 1s
    }, [query]);

    return (
        <div className="relative w-full max-w-md mt-6">
            <input
                type="text"
                placeholder="Search items, mobs, quests..."
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                className="w-full px-4 py-2 bg-black border border-gray-300 rounded shadow"
            />
            {showDropdown && results.length > 0 && (
                <ul className="absolute mt-2 bg-black shadow rounded w-full z-50">
                    {results.map((r, i) => (
                        <li
                            key={i}
                            className={`p-2 hover:bg-gray-700 cursor-pointer ${getRarityClass(r.rarity ?? 1)}`}
                            onClick={() => handleSelect(r)}
                        >
                            {r.type}: {r.name}
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}
