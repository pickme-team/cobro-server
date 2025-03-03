window.jsFunctions = {
    getCameras: async function () {
        if (navigator.mediaDevices && navigator.mediaDevices.enumerateDevices) {
            try {
                const devices = await navigator.mediaDevices.enumerateDevices();
                const cameras = devices.filter(device => device.kind === 'videoinput');
                return cameras.map(camera => ({
                    deviceId: camera.deviceId,
                    label: camera.label || `Camera ${cameras.indexOf(camera) + 1}`
                }));
            } catch (error) {
                console.error("Error getting cameras: ", error);
                return [];
            }
        } else {
            console.error("MediaDevices API or enumerateDevices method is not supported");
            return [];
        }
    },

    startCamera: async function (videoElementId, cameraId) {
        const video = document.getElementById(videoElementId);
        if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
            try {
                const stream = await navigator.mediaDevices.getUserMedia({ video: { deviceId: cameraId ? { exact: cameraId } : undefined } });
                video.srcObject = stream;
                video.play();
                console.log("Camera started successfully.");
            } catch (error) {
                console.error("Error accessing camera: ", error);
            }
        } else {
            console.error("MediaDevices API is not supported");
        }
    },

    stopCamera: function (videoElementId) {
        const video = document.getElementById(videoElementId);
        const stream = video.srcObject;
        const tracks = stream.getTracks();

        tracks.forEach(function (track) {
            track.stop();
        });

        video.srcObject = null;
        console.log("Camera stopped.");
    },

    captureFrame: function (videoElementId, canvasElementId) {
        const video = document.getElementById(videoElementId);
        const canvas = document.getElementById(canvasElementId);
        const context = canvas.getContext('2d');

        if (video.readyState === video.HAVE_ENOUGH_DATA) {
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            context.drawImage(video, 0, 0, canvas.width, canvas.height);
            console.log("Frame captured successfully.");
            return canvas.toDataURL('image/png');
        } else {
            console.error("Video is not ready");
            return null;
        }
    }
};
