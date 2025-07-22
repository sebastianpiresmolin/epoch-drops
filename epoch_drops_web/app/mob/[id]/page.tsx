import Link from "next/link";
import React from "react";

type DroppedItem = {
    itemId: number;
    name: string;
    icon: string;
    rarity: number;
    dropCount: number;
    dropChance: string;
};

type MobDetails = {
    id: number;
    name: string;
    zone: string | null;
    subZone: string | null;
    totalKills: number;
    items: DroppedItem[];
};

export default async function MobPage({
                                          params,
                                      }: {
    params: Promise<{ id: string }>;
}) {
    const { id } = await params;

    const res = await fetch(`http://epoch-drops-production.up.railway.app/mob/${id}`, {
        cache: "no-store",
    });

    if (!res.ok) {
        return <div className="p-6 text-red-500">Mob not found.</div>;
    }

    const data: MobDetails = await res.json();

    return (
        <div className="p-6">
            <a href="/">
                <img src="/full-logo.png" className="w-1/4 p-0 mb-[5vh]"/>
            </a>
            <h1 className="text-2xl font-bold">{data.name}</h1>
            <p className="text-sm mb-4">
                Location: {data.zone ?? "Unknown"}{data.subZone ? ` - ${data.subZone}` : ""}
            </p>
            <p className="text-sm mb-4">
                Total Kills Recorded: {data.totalKills}
            </p>

            <h2 className="text-xl font-semibold mt-6 mb-2">Items Dropped:</h2>
            {data.items.length === 0 ? (
                <p>No item drops recorded.</p>
            ) : (
                <table className="table-auto border border-gray-700 w-full text-sm">
                    <thead>
                    <tr className="bg-gray-800">
                        <th className="px-2 py-1 text-left">Item</th>
                        <th className="px-2 py-1">Count</th>
                        <th className="px-2 py-1">Drop Rate</th>
                    </tr>
                    </thead>
                    <tbody>
                    {data.items.map(item => (
                        <tr key={item.itemId} className="border-t border-gray-600 bg-black">
                            <td className="px-2 py-1">
                                <Link href={`/item/${item.itemId}`} className="flex items-center gap-2 hover:underline">
                                    <img
                                        src={`https://wow.zamimg.com/images/wow/icons/large/${item.icon?.toLowerCase().replace(/ /g, "_")}.jpg`}
                                        alt={item.name}
                                        className="w-6 h-6"
                                    />
                                    <span className={`font-medium ${getRarityColor(item.rarity)}`}>
                                      {item.name}
                                    </span>
                                </Link>
                            </td>
                            <td className="px-2 py-1 text-center">{item.dropCount}</td>
                            <td className="px-2 py-1 text-center">{item.dropChance}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}

function getRarityColor(rarity: number): string {
    switch (rarity) {
        case 0:
            return "text-gray-400";
        case 1:
            return "text-white";
        case 2:
            return "text-green-500";
        case 3:
            return "text-blue-500";
        case 4:
            return "text-purple-500";
        case 5: return "text-orange-500";
        default: return "text-white";
    }
}
