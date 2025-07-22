import { getRarityColor } from '@/lib/rarityUtils';
import Image from 'next/image';
import React from "react";

export default async function QuestPage({
                                            params,
                                        }: {
    params: Promise<{ id: string }>;
}) {
    const { id } = await params;

    const res = await fetch(`https://epoch-drops-production.up.railway.app/quest/${id}`, { cache: 'no-store' });

    if (!res.ok) {
        return <div className="p-6 text-red-500">Quest not found.</div>;
    }

    const data = await res.json();

    return (
        <div className="p-6">
            <a href="/">
                <img src="/full-logo.png" className="w-1/4 p-0 mb-[5vh]"/>
            </a>
            <h1 className="text-2xl font-bold mb-2">{data.title}</h1>

            <div className="mb-6">
                {data.xp > 0 && <p><strong>XP:</strong> {data.xp}</p>}
                {data.money > 0 && <p><strong>Money:</strong> {data.money} copper</p>}
                {data.sourceMobName && <p><strong>Giver:</strong> {data.sourceMobName}</p>}
                {(data.zone || data.subZone) && (
                    <p><strong>Location:</strong> {data.zone}{data.subZone ? ' – ' + data.subZone : ''}</p>
                )}
            </div>

            <h2 className="text-xl font-semibold mt-4 mb-2">Rewards:</h2>
            {data.rewards.length === 0 ? (
                <p>No rewards recorded.</p>
            ) : (
                <table className="table-auto border border-gray-700 w-full text-sm">
                    <thead>
                    <tr className="bg-gray-800">
                        <th className="px-2 py-1 text-left">Item</th>
                        <th className="px-2 py-1 text-center">Count</th>
                    </tr>
                    </thead>
                    <tbody>
                    {data.rewards.map((item: any) => (
                        <tr key={item.itemId} className="border-t border-gray-600 bg-black">
                            <td className="px-2 py-1">
                                <a href={`/item/${item.itemId}`} className="flex items-center gap-2 hover:underline">
                                    <img
                                        src={`https://wow.zamimg.com/images/wow/icons/large/${item.icon?.toLowerCase().replace(/ /g, "_")}.jpg`}
                                        alt={item.itemName}
                                        className="w-6 h-6"
                                    />
                                    <span className={getRarityColor(item.rarity)}>{item.itemName}</span>
                                </a>
                            </td>
                            <td className="px-2 py-1 text-center">{item.count}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}
