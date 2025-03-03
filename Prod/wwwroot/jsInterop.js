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


window.jsFunctions = {
    setLicense: async function setLicense(license) {
        try {
            Dynamsoft.Core.CoreModule.engineResourcePaths.rootDirectory = "https://cdn.jsdelivr.net/npm";
            Dynamsoft.License.LicenseManager.initLicense(license, true);

            await Dynamsoft.Core.CoreModule.loadWasm(["dbr"]);
            
        } catch (e) {
            alert(e);
            return false;
        }

        return true;
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