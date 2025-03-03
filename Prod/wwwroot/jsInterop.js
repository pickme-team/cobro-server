window.jsFunctions = {
    startCamera: async function (videoElementId) {
        const video = document.getElementById(videoElementId);
        if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
            try {
                const stream = await navigator.mediaDevices.getUserMedia({ video: true });
                video.srcObject = stream;
                video.play();
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
    },

    captureFrame: function (videoElementId, canvasElementId) {
        const video = document.getElementById(videoElementId);
        const canvas = document.getElementById(canvasElementId);
        const context = canvas.getContext('2d');

        if (video.readyState === video.HAVE_ENOUGH_DATA) {
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            context.drawImage(video, 0, 0, canvas.width, canvas.height);
            return canvas.toDataURL('image/png');
        } else {
            console.error("Video is not ready");
            return null;
        }
    }
};
