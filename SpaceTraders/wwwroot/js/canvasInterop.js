
function drawWaypoints(canvas, waypoints, scaleFactor, translationX, translationY) {
    if (!canvas.getContext) {
        console.log('Not a canvas element:', canvas);
        return;
    }

    const ctx = canvas.getContext('2d');
    const centerX = canvas.width / 2;
    const centerY = canvas.height / 2;

    ctx.clearRect(0, 0, canvas.width, canvas.height); 

    waypoints.forEach(waypoint => {
        const adjustedX = centerX + (waypoint.x * scaleFactor) + translationX;
        const adjustedY = centerY - (waypoint.y * scaleFactor) + translationY;
        const orbitRadius = Math.sqrt(Math.pow(waypoint.x, 2) + Math.pow(waypoint.y, 2)) * scaleFactor;

        // Draw orbit
        ctx.beginPath();
        ctx.arc(centerX + translationX, centerY + translationY, orbitRadius, 0, Math.PI * 2);
        ctx.strokeStyle = 'gray';
        ctx.setLineDash([5, 5]);
        ctx.stroke();

        // Draw waypoint
        ctx.beginPath();
        // if waypoint type includes "STAR" then fill with first colour in name e.g. ORANGE_STAR
        if (waypoint.type.includes("STAR")) {
            ctx.arc(adjustedX, adjustedY, (10 * scaleFactor), 0, Math.PI * 2);
            ctx.fillStyle = waypoint.type.split("_")[0].toLowerCase();
        } else {
            ctx.arc(adjustedX, adjustedY, (5 * scaleFactor), 0, Math.PI * 2);
            ctx.fillStyle = getColorByType(waypoint.type);
        }
        ctx.fill();

        // Draw text
        ctx.fillStyle = 'white';
        ctx.textAlign = 'center';
        ctx.fillText(waypoint.type, adjustedX, adjustedY + 5);
    });
}

function getColorByType(type) {
    const typeColorMap = {
        "ASTEROID": "gray",
        "FUEL_STATION": "green",
        "ORBITAL_STATION": "blue",
        "PLANET": "brown",
        "MOON": "lightgray"
    };

    return typeColorMap[type] || "red"; 
}