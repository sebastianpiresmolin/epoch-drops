import Link from "next/link";
import { getRarityColor } from "@/lib/rarityUtils";
import React from "react";

type Item = {
    id: number;
    name: string;
    rarity: number;
    icon: string;
};

export default async function CategoryPage({ params, searchParams }: {
    params: { subtype: string };
    searchParams: { page?: string };
}) {
    const page = parseInt(searchParams.page ?? "1", 10);
    const res = await fetch(`http://localhost:5223/items/by-subtype?subType=${params.subtype}&page=${page}`, {
        cache: "no-store"
    });

    if (!res.ok) return <p className="text-red-500">Failed to load items.</p>;

    const { items, totalPages } = await res.json();

    return (
        <div className="p-6 text-white">
            <a href="/">
                <img src="/full-logo.png" className="w-1/4 p-0 mb-[5vh]"/>
            </a>
            <h1 className="text-2xl font-bold mb-4">{decodeURIComponent(params.subtype)}</h1>
            <p className="text-sm mb-6">Sorted by rarity, paginated by 50</p>
            <table className="table-auto border border-gray-700 w-full text-sm">
                <thead>
                <tr className="bg-gray-800">
                    <th className="px-2 py-1 text-left">Item</th>
                </tr>
                </thead>
                <tbody>
                {items.map((item: Item) => (
                    <tr key={item.id} className="border-t border-gray-600 bg-black">
                        <td className="px-2 py-1">
                            <Link href={`/item/${item.id}`} className="flex items-center gap-2 hover:underline">
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
                    </tr>
                ))}
                </tbody>
            </table>

            <div className="mt-6 flex gap-4">
                {page > 1 && (
                    <Link
                        className="bg-gray-800 px-4 py-2 rounded hover:bg-gray-700"
                        href={`?page=${page - 1}`}
                    >
                        Previous
                    </Link>
                )}
                {page < totalPages && (
                    <Link
                        className="bg-gray-800 px-4 py-2 rounded hover:bg-gray-700"
                        href={`?page=${page + 1}`}
                    >
                        Next
                    </Link>
                )}
            </div>
        </div>
    );
}
