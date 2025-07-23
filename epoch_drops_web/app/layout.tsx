import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import { Analytics } from "@vercel/analytics/next"

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Epoch Drops",
  description: "Community based database",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
    <body className={`${geistSans.variable} relative`}>
    <div className="absolute min-w-full min-h-screen inset-0 z-0 bg-[url('/bg.jpeg')] bg-cover bg-center opacity-50"/>
    <div className="relative z-10">
      {children}
      <Analytics />
    </div>
    </body>
    </html>
  );
}
