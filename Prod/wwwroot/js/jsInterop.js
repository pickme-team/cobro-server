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
    },
    
    initReader: async function () {
        try {
            dispose();
            cvr = await Dynamsoft.CVR.CaptureVisionRouter.createInstance();

        } catch (e) {
            console.log(e);
        }
    },

    selectFile: async function (dotnetRef, overlayId, imageId) {
        if (cameraEnhancer) {
            cameraEnhancer.dispose();
            cameraEnhancer = null;
        }
        initOverlay(document.getElementById(overlayId));
        if (cvr) {
            let input = document.createElement("input");
            input.type = "file";
            input.onchange = async function () {
                try {
                    let file = input.files[0];
                    var fr = new FileReader();
                    fr.onload = function () {
                        let image = document.getElementById(imageId);
                        image.src = fr.result;
                        image.style.display = 'block';

                        decodeImage(dotnetRef, fr.result, file);
                    }
                    fr.readAsDataURL(file);

                } catch (ex) {
                    alert(ex.message);
                    throw ex;
                }
            };
            input.click();
        } else {
            alert("The barcode reader is still initializing.");
        }
    },

    initScanner: async function (dotnetRef, videoId, selectId, overlayId) {
        let canvas = document.getElementById(overlayId);
        initOverlay(canvas);
        videoSelect = document.getElementById(selectId);
        videoSelect.onchange = openCamera;
        try {
            dispose();

            let cameraView = await Dynamsoft.DCE.CameraView.createInstance();
            cameraEnhancer = await Dynamsoft.DCE.CameraEnhancer.createInstance(cameraView);

            let uiElement = document.getElementById(videoId);
            uiElement.append(cameraView.getUIElement());

            cameraView.getUIElement().shadowRoot?.querySelector('.dce-sel-camera')?.setAttribute('style', 'display: none');
            cameraView.getUIElement().shadowRoot?.querySelector('.dce-sel-resolution')?.setAttribute('style', 'display: none');

            cvr = await Dynamsoft.CVR.CaptureVisionRouter.createInstance();
            cvr.setInput(cameraEnhancer);

            cvr.addResultReceiver({
                onCapturedResultReceived: (result) => {
                    showResults(result, dotnetRef);
                },
            });

            cvr.addResultReceiver({
                onDecodedBarcodesReceived: (result) => {
                    if (!result.barcodeResultItems.length) return;

                },
            });

            cameraEnhancer.on('played', () => {
                updateResolution();
            });

        } catch (e) {
            console.log(e);
            result = false;
        }
        return true;
    },
    getCameras: async function () {
        if (cameraEnhancer) {
            let cameras = await cameraEnhancer.getAllCameras();
            listCameras(cameras);
        }
    },
    startCamera: async function() {
        openCamera();
    },
    stopCamera: async function () {
        try {
            if (cameraEnhancer) {
                cameraEnhancer.pause();
            }
        }
        catch (e) {
            console.log(e);
        }
    }
};

function showResults(result, dotnetRef) {
    clearOverlay();

    let txts = [];
    try {
        let localization;
        let items = result.items
        if (items.length > 0) {
            for (var i = 0; i < items.length; ++i) {

                if (items[i].type !== Dynamsoft.Core.EnumCapturedResultItemType.CRIT_BARCODE) {
                    continue;
                }

                let item = items[i];

                txts.push(item.text);
                localization = item.location;

                drawOverlay(
                    localization,
                    item.text
                );
            }

        }
    } catch (e) {
        alert(e);
    }

    let barcoderesults = txts.join(', ');
    if (txts.length == 0) {
        barcoderesults = 'No barcode found';
    }

    if (dotnetRef) {
        dotnetRef.invokeMethodAsync('ReturnBarcodeResultsAsync', barcoderesults);
    }
}

function decodeImage(dotnetRef, url, data) {
    const img = new Image()
    img.onload = () => {
        updateOverlay(img.width, img.height);
        if (cvr) {
            cvr.capture(url, 'ReadBarcodes_Balance').then((result) => {
                showResults(result, dotnetRef);
            });

        }
    }
    img.src = url
}

async function openCamera() {
    clearOverlay();

    try {
        let deviceId = videoSelect.value;
        if (cameraEnhancer && deviceId !== "") {
            await cameraEnhancer.selectCamera(deviceId);
            await cameraEnhancer.open();
            cvr.startCapturing('ReadSingleBarcode');
        }
    }
    catch(e) {
        console.log(e);
    }
}