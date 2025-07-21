import SearchBar from "@/components/SearchBar";


export default function Home() {
  return (
      <div className="flex flex-col items-center justify-center p-6 pt-[20vh]">
          <img src="/full-logo.png" className="w-1/4"/>
          <SearchBar />
      </div>
  );
}
