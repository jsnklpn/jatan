
// Point class
function Point(x, y) {
    this.x = x;
    this.y = y;
    this.subtract = function(p2) {
        return new Point(x - p2.x, y - p2.y);
    };
    this.add = function (p2) {
        return new Point(x + p2.x, y + p2.y);
    };
    this.multiply = function(scalar) {
        return new Point(x * scalar, y * scalar);
    };
    this.dotProduct = function(p2) {
        return new Point(x * p2.x + y * p2.y);
    };
    this.distance = function(p2) {
        var d = subtract(p2);
        return Math.sqrt(d.dotProduct(d));
    };
    // segment is of type 'Line'
    this.distanceToSegment = function(segment) {
        var v = segment.p2.subtract(segment.p1);
        var w = subtract(segment.p1);
        var c1 = w.dotProduct(v);
        if (c1 <= 0) return distance(segment.p1);
        var c2 = v.dotProduct(v);
        if (c2 <= c1) return distance(segment.p2);
        var b = c1 / c2;
        var pb = segment.p1.add(v.multiply(b));
        return distance(pb);
    };
}

// Line class
function Line(p1, p2) {
    this.p1 = p1;
    this.p2 = p2;
    this.length = p1.distance(p2);
}