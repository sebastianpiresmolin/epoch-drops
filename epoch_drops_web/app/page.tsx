import SearchBar from "@/components/SearchBar";
import CategoryFilter from "@/components/CategoryFilter";


export default function Home() {
  return (
      <div className="flex flex-col items-center justify-center p-6 pt-[20vh]">
          <img src="/full-logo.png" className="w-1/4"/>
          <h1 className="font-bold text-xl">Unoffical Community Item Database</h1>
          <SearchBar/>
          <CategoryFilter/>
          <h2 className="font-bold mt-2">Want to help gather data? Download the addon <a href="https://github.com/sebastianpiresmolin/epoch-drops/blob/main/README.md"
                                                                                         className="text-blue-400">here</a>
          </h2>
          <h1 className="pt-6 text-xl text-green-500 font-bold">NEW WORKING ADDON 2025-08-17, please update on link above</h1>
      </div>
  );
}
