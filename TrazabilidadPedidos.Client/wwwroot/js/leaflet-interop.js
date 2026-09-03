window.leafletInterop = {
    _maps: {},

    initMap: function (containerId, lat, lng, zoom, dotNetRef, callbackMethodName) {
        if (this._maps[containerId]) {
            this._maps[containerId].remove();
        }

        var map = L.map(containerId, {
            center: [lat, lng],
            zoom: zoom,
            maxBounds: [[-18.0, -63.5], [-17.5, -62.8]],
            minZoom: 11,
            maxZoom: 18
        });

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors',
            maxZoom: 19
        }).addTo(map);

        var marker = L.marker([lat, lng], { draggable: true }).addTo(map);

        marker.on('dragend', function (e) {
            var pos = e.target.getLatLng();
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync(callbackMethodName, pos.lat, pos.lng);
            }
        });

        map.on('click', function (e) {
            marker.setLatLng(e.latlng);
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync(callbackMethodName, e.latlng.lat, e.latlng.lng);
            }
        });

        this._maps[containerId] = map;
        this._maps[containerId + '_marker'] = marker;

        setTimeout(function () { map.invalidateSize(); }, 200);

        return true;
    },

    updateMarker: function (containerId, lat, lng) {
        var map = this._maps[containerId];
        var marker = this._maps[containerId + '_marker'];
        if (map && marker) {
            marker.setLatLng([lat, lng]);
            map.setView([lat, lng], map.getZoom());
        }
    },

    initReadOnlyMap: function (containerId, lat, lng, zoom) {
        if (this._maps[containerId]) {
            this._maps[containerId].remove();
        }

        var map = L.map(containerId, {
            center: [lat, lng],
            zoom: zoom,
            dragging: false,
            scrollWheelZoom: false,
            doubleClickZoom: false
        });

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);

        L.marker([lat, lng]).addTo(map);

        this._maps[containerId] = map;
        setTimeout(function () { map.invalidateSize(); }, 200);

        return true;
    },

    destroyMap: function (containerId) {
        if (this._maps[containerId]) {
            this._maps[containerId].remove();
            delete this._maps[containerId];
            delete this._maps[containerId + '_marker'];
        }
    }
};

window.qrInterop = {
    generateQR: function (containerId, text, size) {
        var el = document.getElementById(containerId);
        if (!el) return false;
        el.innerHTML = '';
        new QRCode(el, {
            text: text,
            width: size || 200,
            height: size || 200,
            colorDark: "#1a237e",
            colorLight: "#ffffff",
            correctLevel: QRCode.CorrectLevel.H
        });
        return true;
    }
};

window.downloadFile = function (fileName, bytes) {
    var blob = new Blob([new Uint8Array(bytes)], { type: "application/pdf" });
    var url = URL.createObjectURL(blob);
    var a = document.createElement("a");
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};

window.chartInterop = {
    _charts: {},

    createBarChart: function (canvasId, labels, data, title) {
        var ctx = document.getElementById(canvasId);
        if (!ctx) return false;
        if (this._charts[canvasId]) this._charts[canvasId].destroy();
        this._charts[canvasId] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: title,
                    data: data,
                    backgroundColor: ['#1a237e', '#3949ab', '#5c6bc0', '#7986cb', '#9fa8da', '#c5cae9'],
                    borderRadius: 6
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: { y: { beginAtZero: true, ticks: { stepSize: 1 } } }
            }
        });
        return true;
    },

    createPieChart: function (canvasId, labels, data) {
        var ctx = document.getElementById(canvasId);
        if (!ctx) return false;
        if (this._charts[canvasId]) this._charts[canvasId].destroy();
        this._charts[canvasId] = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: ['#1a237e', '#3949ab', '#f44336', '#ff9800', '#4caf50', '#9c27b0'],
                    borderWidth: 2,
                    borderColor: '#ffffff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'bottom', labels: { padding: 12 } }
                }
            }
        });
        return true;
    },

    destroyChart: function (canvasId) {
        if (this._charts[canvasId]) {
            this._charts[canvasId].destroy();
            delete this._charts[canvasId];
        }
    }
};
