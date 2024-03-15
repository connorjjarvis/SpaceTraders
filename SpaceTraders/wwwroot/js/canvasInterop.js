window.drawOnCanvas = function (canvas, objects, scaleFactor, translationX, translationY) {
    if (!canvas.getContext) {
        return; // Canvas not supported
    }

    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height); // Clear previous drawings

    objects.forEach(obj => {
        const adjustedX = (canvas.width / 2) + (obj.x * scaleFactor) + translationX;
        const adjustedY = (canvas.height / 2) - (obj.y * scaleFactor) + translationY; // Invert Y-axis

        // Draw orbit
        const orbitRadius = Math.sqrt(obj.x ** 2 + obj.y ** 2) * scaleFactor;
        ctx.beginPath();
        ctx.arc(canvas.width / 2 + translationX, canvas.height / 2 + translationY, orbitRadius, 0, 2 * Math.PI);
        ctx.strokeStyle = 'gray';
        ctx.setLineDash([5, 5]);
        ctx.stroke();

        // Draw waypoint
        ctx.beginPath();
        ctx.arc(adjustedX, adjustedY, 5, 0, 2 * Math.PI);
        ctx.fillStyle = obj.color; // Assuming color is determined beforehand
        ctx.fill();

        // Draw label
        ctx.fillStyle = 'white';
        ctx.textAlign = 'center';
        ctx.fillText(obj.type, adjustedX, adjustedY + 20); // Adjust label position as needed
    });
};