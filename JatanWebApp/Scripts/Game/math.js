//=========================
// Classes
//=========================

// Point class
function Point(x, y) {
    this.x = x;
    this.y = y;
    this.toString = function() {
        return "(" + this.x + ", " + this.y + ")";
    }
    this.subtract = function(p2) {
        return new Point(this.x - p2.x, this.y - p2.y);
    };
    this.add = function (p2) {
        return new Point(this.x + p2.x, this.y + p2.y);
    };
    this.multiply = function(scalar) {
        return new Point(this.x * scalar, this.y * scalar);
    };
    this.dotProduct = function(p2) {
        return this.x * p2.x + this.y * p2.y;
    };
    this.distance = function(p2) {
        var d = this.subtract(p2);
        return Math.sqrt(d.dotProduct(d));
    };
    // segment is of type 'Line'
    this.distanceToSegment = function(segment) {
        var v = segment.p2.subtract(segment.p1);
        var w = this.subtract(segment.p1);
        var c1 = w.dotProduct(v);
        if (c1 <= 0) return this.distance(segment.p1);
        var c2 = v.dotProduct(v);
        if (c2 <= c1) return this.distance(segment.p2);
        var b = c1 / c2;
        var pb = segment.p1.add(v.multiply(b));
        return this.distance(pb);
    };
}

// Line class
function Line(p1, p2) {
    this.p1 = p1;
    this.p2 = p2;
    this.length = p1.distance(p2);
}

// Cube class
function Cube(x, y, z) {
    this.x = x;
    this.y = y;
    this.z = z;
    this.round = function() {
        var rx = Math.round(this.x);
        var ry = Math.round(this.x);
        var rz = Math.round(this.x);
        return new Cube(rx, ry, rz);
    }
}

function Rect(x, y, w, h) {
    this.x = x;
    this.y = y;
    this.width = w;
    this.height = h;
    this.center = function() {
        return new Point(this.x + (this.width / 2), this.y + (this.height / 2));
    }
}

//=========================
// Helper functions
//=========================

// Converts "(x, y)" string to a Point class.
function stringToPoint(str) {
    str = str.slice(1, -1);
    var split = str.split(", ");
    return new Point(parseFloat(split[0]), parseFloat(split[1]));
}

function getBoundingBox(points) {
    var xMax = Number.MIN_VALUE;
    var yMax = Number.MIN_VALUE;
    var xMin = Number.MAX_VALUE;
    var yMin = Number.MAX_VALUE;
    for (var i = 0; i < points.length; i++) {
        var p = points[i];
        if (p.x > xMax) xMax = p.x;
        if (p.y > yMax) yMax = p.y;
        if (p.x < xMin) xMin = p.x;
        if (p.y < yMin) yMin = p.y;
    }
    var width = xMax - xMin;
    var height = yMax - yMin;
    var x = xMin;
    var y = yMin;
    return new Rect(x, y, width, height);
}