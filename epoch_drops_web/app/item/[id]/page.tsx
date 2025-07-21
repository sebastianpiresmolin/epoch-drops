import { notFound } from "next/navigation";

type Props = {
    params: {
        id: string;
    };
};

export default async function ItemPage({ params }: Props) {
    const res = await fetch(`http://localhost:5223/item/${params.id}`, {
        cache: "no-store",
    });

    if (!res.ok) return notFound();

    const item = await res.json();

    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold">{item.name}</h1>
            <p>Type: {item.itemType} - {item.itemSubType}</p>
            <p>Rarity: {item.rarity}</p>
            <p>Equip slot: {item.equipSlot}</p>
            <div className="mt-4">
        <pre className="text-sm bg-gray-100 p-2 rounded">
          {item.tooltip.join("\n")}
        </pre>
            </div>
        </div>
    );
}
