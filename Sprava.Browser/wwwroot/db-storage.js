export async function saveDatabase(fileName, byteArrays) {
    const db = await openDB();
    const transaction = db.transaction(["files"], "readwrite");
    const store = transaction.objectStore("files");
    await store.put(new Blob([byteArrays]), fileName);
}

export async function loadDatabase(fileName) {
    const db = await openDB();

    return new Promise((resolve) => {
        const transaction = db.transaction(["files"], "readonly");
        const store = transaction.objectStore("files");
        const request = store.get(fileName);

        request.onsuccess = async () => {
            const blob = request.result;
            if (blob) {
                const buf = await blob.arrayBuffer();
                const bytes = new Uint8Array(buf);

                let binary = "";
                for (let i = 0; i < bytes.length; i++) {
                    binary += String.fromCharCode(bytes[i]);
                }

                resolve(btoa(binary));
            } else {
                resolve("");
            }
        };

        request.onerror = () => resolve("");
    });
}

function openDB() {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open("UltraLiteStorage", 1);
        request.onupgradeneeded = () => request.result.createObjectStore("files");
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
}