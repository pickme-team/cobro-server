let currentStream = null; // Храним текущий поток

window.jsFunctions = {
    getCameras: async function () {
        const devices = await navigator.mediaDevices.enumerateDevices();
        const videoDevices = devices.filter(device => device.kind === 'videoinput');
        return videoDevices;
    },

    startCamera: async function (videoElementId, deviceId = null) {
        const videoElement = document.getElementById(videoElementId);
        const constraints = {
            video: deviceId ? { deviceId: { exact: deviceId } } : true
        };

        if (currentStream) {
            currentStream.getTracks().forEach(track => track.stop());
        }

        const stream = await navigator.mediaDevices.getUserMedia(constraints);
        videoElement.srcObject = stream;
        await videoElement.play();
        currentStream = stream;
    },

    stopCamera: function (videoElementId) {
        const videoElement = document.getElementById(videoElementId);
        if (currentStream) {
            currentStream.getTracks().forEach(track => track.stop());
            currentStream = null;
        }
        videoElement.srcObject = null;
    },

    captureFrame: async function () {
        if (!currentStream) {
            throw new Error("Camera stream is not available.");
        }

        const track = currentStream.getVideoTracks()[0];
        const imageCapture = new ImageCapture(track);
        const blob = await imageCapture.takePhoto();
        const arrayBuffer = await blob.arrayBuffer();
        return Array.from(new Uint8Array(arrayBuffer));
    }
};