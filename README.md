# How to install addon

*Download Epoch_Drops and place the entire folder in your Addons folder:*

https://drive.google.com/drive/folders/1Pd0qnETjdFyONBkO_0ldmX3OV4FGZUT7?usp=drive_link

*Download EpochDropsUploader and place the folder wherever you want:*

https://drive.google.com/drive/folders/16phXHPQ1df6tmI-1ffAUDee9M9s4npso?usp=drive_link

*Inside the EpochDropsUploader, run the .exe and the first time running you should be prompted to select the location of your Epoch wow installation*

*If you're not prompted, just right click the Epoch icon in the sytem tray and select Change installation path*
<img width="448" height="155" alt="image" src="https://github.com/user-attachments/assets/d74b7214-9339-4d6f-ab4a-0d63dcce6c95" />

**Now you're ready to go! Each time you wanna upload, play the game with the exe running. Don't worry if you forgot to start the exe when playing. The data will be saved and uploaded the next time you run the game with the exe running**

# Epoch Drops

> A comprehensive WoW 3.3.5a item drop tracker and explorer for the Epoch server.  
> Consists of a WoW Addon, an uploader tray application, a .NET API backend, and a modern frontend web app.

---

## 🌐 Overview

Epoch Drops is a multi-component ecosystem built to collect, analyze, and display mob drop and quest reward data from World of Warcraft (3.3.5a), focused specifically on the **Epoch** server.

This project helps players discover where to find items, how often they drop, and what quests reward them.

---

## 🧩 Project Structure

### 🧙‍♂️ Addon: `Epoch_Drops`

A lightweight WoW addon written in Lua that:

- Tracks item drops from mobs and quest rewards.
- Serializes collected data into a JSON-encoded Lua string.
- Stores data in the `SavedVariables` file for the tray app to access.

### 🖥️ Tray App: `EpochDropsUploader`

A Windows-only tray application built with C# WinForms that:

- Runs silently in the background from the system tray.
- Watches all relevant `SavedVariables` folders for the `Epoch_Drops.lua` file.
- Validates that the addon is collecting from the correct realm.
- Extracts and parses JSON data from the Lua file.
- Uploads data securely to the backend API using a secret upload key.
- Renames uploaded files to prevent duplicates.

### 🛠️ API Backend: `EpochDropsAPI`

A fast and minimal REST API built with **ASP.NET Core Minimal API**:

- Accepts uploads of parsed addon data (`/upload`).
- Provides endpoints for:
  - Items by ID, category, and subtype.
  - Mobs by ID.
  - Quests and their associated rewards.
  - Quick search.
- Stores all data in **PostgreSQL**.
- Handles CORS for the frontend.
- Automatically applies database migrations at startup.
- Docker-ready and deployed via Railway.

### 🌍 Web App: `epoch-drops.com`

A modern web frontend built with **Next.js App Router**:

- Displays a searchable, categorized list of all items.
- Shows item detail pages with:
  - Drop rate data based on uploads.
  - Quest reward associations.
- Includes mob and quest detail pages.
- Fully responsive and hosted on **Vercel**.
- Connects to the API for real-time data.

---

## 🧾 Tech Stack

| Component        | Technology                          |
|------------------|--------------------------------------|
| Addon            | Lua (WoW 3.3.5a AddOn)               |
| Tray App         | C# WinForms                          |
| Backend API      | ASP.NET Core Minimal API + PostgreSQL |
| Web Frontend     | Next.js (App Router) + Tailwind CSS  |
| Hosting          | Railway (API + DB), Vercel (Web)     |

---

## 📁 Repositories Structure

/EpochDropsRoot/
│
├── Epoch_Drops/ # Lua Addon
├── EpochDropsUploader/ # Windows tray app
├── EpochDropsAPI/ # ASP.NET Core backend
└── epoch-drops-web/ # Next.js frontend

---

## 📌 Notes

- **Data is only accepted from ChromieCraft** (or the configured realm).
- **Secure uploads** using a hardcoded key embedded in the tray app.
- **No user accounts or authentication required** — it's designed to be seamless and automatic.

---

## 🛠 How to Use

*You can fill this section in yourself, detailing how to install the addon, run the tray app, and explore the site.*

---

## 🧠 Future Improvements

- Web-based submission for Linux/macOS users.
- Authentication for uploader uploads (instead of just a secret key).
- Admin panel for moderation or data cleanup.
- Automatic addon version checking and updates.

---

## © License

MIT License – Feel free to use, modify, and contribute.
