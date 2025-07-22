import { getRarityColor } from '@/lib/rarityUtils';
import Link from 'next/link';

interface Item {
    id: number;
    name: string;
    icon: string;
    rarity: number;
}

interface PageProps {
    params: { subtype: string; slot: string };
    searchParams: { page?: string };
}

export default async function ArmorSlotPage({ params, searchParams }: PageProps) {
    const subtype = decodeURIComponent(params.subtype);
    const slot = decodeURIComponent(params.slot);
    const page = searchParams.page || '1';

    const res = await fetch(`http://localhost:5223/category/armor?subtype=${subtype}&slot=${slot}&page=${page}`, {
        cache: 'no-store',
    });

    if (!res.ok) {
        return <div className="p-6 text-red-500">Failed to load items.</div>;
    }

    const data = await res.json() as {
        items: Item[];
        hasPrevious: boolean;
        hasNext: boolean;
    };

    return (
        <div className="p-6">
            <a href="/">
                <img src="/full-logo.png" className="w-1/4 p-0 mb-[5vh]" />
            </a>

            <h1 className="text-2xl font-bold mb-2">
                Armor: {subtype} – {slot}
            </h1>
            <p className="text-sm mb-6">Sorted by rarity, paginated by 50</p>

            {data.items.length === 0 ? (
                <p>No items found.</p>
            ) : (
                <table className="table-auto border border-gray-700 w-full text-sm">
                    <thead>
                    <tr className="bg-gray-800">
                        <th className="px-2 py-1 text-left">Item</th>
                        <th className="px-2 py-1 text-center">Rarity</th>
                    </tr>
                    </thead>
                    <tbody>
                    {data.items.map(item => (
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
                            <td className="px-2 py-1 text-center">{item.rarity}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            )}

            <div className="mt-6 flex justify-center gap-4">
                {data.hasPrevious && (
                    <Link href={`?page=${+page - 1}`} className="px-4 py-2 bg-gray-800 border border-white rounded hover:bg-gray-700">
                        Previous
                    </Link>
                )}
                {data.hasNext && (
                    <Link href={`?page=${+page + 1}`} className="px-4 py-2 bg-gray-800 border border-white rounded hover:bg-gray-700">
                        Next
                    </Link>
                )}
            </div>
        </div>
    );
}
