window.jsFunctions = {
    setLicense: async function setLicense(license) {
        try {
            Dynamsoft.Core.CoreModule.engineResourcePaths = {
                std: "https://cdn.jsdelivr.net/npm/dynamsoft-capture-vision-std@1.2.10/dist/",
                dip: "https://cdn.jsdelivr.net/npm/dynamsoft-image-processing@2.2.30/dist/",
                core: "https://cdn.jsdelivr.net/npm/dynamsoft-core@3.2.30/dist/",
                license: "https://cdn.jsdelivr.net/npm/dynamsoft-license@3.2.21/dist/",
                cvr: "https://cdn.jsdelivr.net/npm/dynamsoft-capture-vision-router@2.2.30/dist/",
                dce: "https://cdn.jsdelivr.net/npm/dynamsoft-camera-enhancer@4.0.3/dist/",
                dbr: "https://cdn.jsdelivr.net/npm/dynamsoft-barcode-reader@10.2.10/dist/",
                dlr: "https://cdn.jsdelivr.net/npm/dynamsoft-label-recognizer@3.2.30/dist/",
                dcp: "https://cdn.jsdelivr.net/npm/dynamsoft-code-parser@2.2.10/dist/",
                ddn: "https://cdn.jsdelivr.net/npm/dynamsoft-document-normalizer@2.2.10/dist/",
            };
            Dynamsoft.Core.CoreModule.loadWasm(["dbr"]);
            Dynamsoft.License.LicenseManager.initLicense(license, true);
            await initSDK();
        } catch (e) {
            console.log(e);
            return false;
        }

        return true;
    }
};