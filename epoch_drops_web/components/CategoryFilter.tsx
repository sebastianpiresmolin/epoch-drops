'use client';
import { useState } from 'react';
import Link from 'next/link';

type WeaponCategory =
    | "One-Handed Axes" | "Two-Handed Axes" | "Bows" | "Guns"
    | "One-Handed Maces" | "Two-Handed Maces" | "Polearms"
    | "One-Handed Swords" | "Two-Handed Swords" | "Staves"
    | "Fist Weapons" | "Miscellaneous" | "Daggers" | "Thrown"
    | "Crossbows" | "Wands" | "Fishing Poles";

type ArmorSlot = "Head" | "Shoulder" | "Chest" | "Wrist" | "Hands" | "Waist" | "Legs" | "Feet";
type MiscSubType = "Amulets" | "Cloaks" | "Rings" | "Trinkets" | "Off-hand" | "Shirts" | "Tabards";

type ArmorCategory = {
    Cloth: ArmorSlot[];
    Leather: ArmorSlot[];
    Mail: ArmorSlot[];
    Plate: ArmorSlot[];
    Shields: string[];
    Librams: string[];
    Idols: string[];
    Totems: string[];
    Miscellaneous: MiscSubType[];
};

type Categories = {
    Weapons: WeaponCategory[];
    Armor: ArmorCategory;
    Containers: string[];
    Consumables: string[];
    Miscellaneous: string[];
    Quest: string[];
    Keys: string[];
};

const categories: Categories = {
    Weapons: [
        "One-Handed Axes", "Two-Handed Axes", "Bows", "Guns",
        "One-Handed Maces", "Two-Handed Maces", "Polearms",
        "One-Handed Swords", "Two-Handed Swords", "Staves",
        "Fist Weapons", "Miscellaneous", "Daggers", "Thrown",
        "Crossbows", "Wands", "Fishing Poles",
    ],
    Armor: {
        Cloth: ["Head", "Shoulder", "Chest", "Wrist", "Hands", "Waist", "Legs", "Feet"],
        Leather: ["Head", "Shoulder", "Chest", "Wrist", "Hands", "Waist", "Legs", "Feet"],
        Mail: ["Head", "Shoulder", "Chest", "Wrist", "Hands", "Waist", "Legs", "Feet"],
        Plate: ["Head", "Shoulder", "Chest", "Wrist", "Hands", "Waist", "Legs", "Feet"],
        Shields: [],
        Librams: [],
        Idols: [],
        Totems: [],
        Miscellaneous: ["Amulets", "Cloaks", "Rings", "Trinkets", "Off-hand", "Shirts", "Tabards"]
    },
    Containers: [],
    Consumables: [],
    Miscellaneous: [],
    Quest: [],
    Keys: []
};

export default function CategoryFilter() {
    const [hoveredMain, setHoveredMain] = useState<string | null>(null);
    const [hoveredSub, setHoveredSub] = useState<string | null>(null);

    const renderNested = (label: string) => {
        const sub = categories[label as keyof Categories];
        if (!sub) return null;

        // If sub is an empty array, render a direct link
        if (Array.isArray(sub)) {
            if (sub.length === 0) {
                return (
                    <div className="absolute left-full top-0 bg-gray-900 border border-white p-2 w-64 text-sm text-white">
                        <Link
                            href={`/category/general?category=${encodeURIComponent(label)}`}
                            className="block hover:bg-gray-700 px-2 py-1"
                        >
                            {label}
                        </Link>
                    </div>
                );
            }

            // Render submenu list
            return (
                <div className="absolute left-full top-0 bg-gray-900 border border-white p-2 w-64 text-sm text-white">
                    {sub.map((v) => (
                        <Link
                            key={v}
                            href={`/category/${encodeURIComponent(v)}`}
                            className="block hover:bg-gray-700 px-2 py-1"
                        >
                            {v}
                        </Link>
                    ))}
                </div>
            );
        }

        // For Armor (object structure)
        if (typeof sub === 'object') {
            return (
                <div className="absolute left-full top-0 bg-gray-900 border border-white p-2 w-64 text-sm text-white">
                    {Object.entries(sub).map(([key, val]) => (
                        <div
                            key={key}
                            className="relative group px-2 py-1 hover:bg-gray-700 cursor-pointer"
                            onMouseEnter={() => setHoveredSub(key)}
                        >
                            {key}
                            {hoveredSub === key && (
                                Array.isArray(val) && val.length > 0 ? (
                                    <div className="absolute left-full top-0 bg-gray-800 border border-white p-2 w-48">
                                        {val.map((slot) => (
                                            <Link
                                                key={slot}
                                                href={`/armor/${encodeURIComponent(key)}/${encodeURIComponent(slot)}`}
                                                className="block hover:bg-gray-700 px-2 py-1"
                                            >
                                                {slot}
                                            </Link>
                                        ))}
                                    </div>
                                ) : (
                                    <Link
                                        href={`/armor/${encodeURIComponent(key)}/${encodeURIComponent(key)}`}
                                        className="absolute left-full top-0 bg-gray-800 border border-white px-2 py-1 w-48 hover:bg-gray-700 block"
                                    >
                                        View {key}
                                    </Link>
                                )
                            )}
                        </div>
                    ))}
                </div>
            );
        }

        return null;
    };



    return (
        <div
            className="relative mt-2 text-white text-sm"
            onMouseLeave={() => {
                setHoveredMain(null);
                setHoveredSub(null);
            }}
        >
            <div
                className="inline-block px-4 py-2 bg-black border border-white rounded cursor-pointer"
                onMouseEnter={() => setHoveredMain('root')}
            >
                Browse Categories
            </div>

            {hoveredMain && (
                <div className="absolute top-full left-0 bg-gray-900 border border-white p-2 w-64 z-50">
                    {Object.keys(categories).map((label) => (
                        <div
                            key={label}
                            className="relative group px-2 py-1 hover:bg-gray-700 cursor-pointer"
                            onMouseEnter={() => {
                                setHoveredMain(label);
                                setHoveredSub(null);
                            }}
                        >
                            {label}
                            {hoveredMain === label && renderNested(label)}
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}
