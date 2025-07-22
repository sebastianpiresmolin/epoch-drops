import Link from "next/link";
import {getRarityColor} from "@/lib/rarityUtils";

export default async function GeneralCategoryPage({
                                                      searchParams,
                                                  }: {
    searchParams: { category?: string; page?: string };
}) {
    const category = decodeURIComponent(searchParams.category ?? "");
    const page = parseInt(searchParams.page ?? "1", 10);

    const res = await fetch(
        `http://localhost:5223/category/general?category=${category}&page=${page}`,
        { cache: "no-store" }
    );

    if (!res.ok) {
        return <p className="text-red-500">Failed to load items.</p>;
    }

    const { items, totalPages } = await res.json();

    return (
        <div className="p-6 text-white">
            <a href="/">
                <img src="/full-logo.png" className="w-1/4 p-0 mb-[5vh]"/>
            </a>
            <h1 className="text-2xl font-bold mb-4">{category}</h1>
            <table className="table-auto border border-gray-700 w-full text-sm">
                <thead>
                <tr className="bg-gray-800">
                    <th className="px-2 py-1 text-left">Item</th>
                </tr>
                </thead>
                <tbody>
                {items.map((item: any) => (
                    <tr key={item.id} className="border-t border-gray-600 bg-black">
                        <td className="px-2 py-1">
                            <Link
                                href={`/item/${item.id}`}
                                className="flex items-center gap-2 hover:underline"
                            >
                                <img
                                    src={`https://wow.zamimg.com/images/wow/icons/large/${item.icon?.toLowerCase().replace(/ /g, "_")}.jpg`}
                                    className="w-6 h-6"
                                    alt={item.name}
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

            <div className="mt-6 flex justify-center gap-4">
                {page > 1 && (
                    <Link
                        href={`?category=${encodeURIComponent(category)}&page=${page - 1}`}
                        className="bg-gray-800 px-4 py-2 rounded hover:bg-gray-700"
                    >
                        ← Previous
                    </Link>
                )}
                <span className="text-gray-400">Page {page} of {totalPages}</span>
                {page < totalPages && (
                    <Link
                        href={`?category=${encodeURIComponent(category)}&page=${page + 1}`}
                        className="bg-gray-800 px-4 py-2 rounded hover:bg-gray-700"
                    >
                        Next →
                    </Link>
                )}
            </div>
        </div>
    );
}
