//convert AST JSON into a tree
function jsontoTree(json) {
    // Check if json is an object and has the 'Case' property
    if (json && json.Case) {
        return {
            name: json.Case,  // Use the 'Case' value for the node's name
            children: (json.Fields || []).map(function(field) {
                if (typeof field === 'object') {
                    // Recursively process if it's an object
                    return jsontoTree(field);
                } else {
                    // else treat it as a leaf node with 'name' being the field value
                    return { name: field.toString() };
                }
            })
        };
    } else {
        // In case it's not an object, return it as a leaf node
        return { name: json.toString() };
    }
}


function drawTree(astData) {
    // Check if astData is a string, and parse it if so
    if (typeof astData === 'string') {
        astData = JSON.parse(astData);  // Parse the JSON string into an object
    }
    //Convert the AST Json into a format that D3 can use
    var root = jsontoTree(astData);

    //set up the dimensons and margins for the tree 
    var margin = { top: 20, right: 90, bottom:30, left: 90},
    width = 960 - margin.left - margin.right,
    height = 500 - margin.top - margin.bottom;

    var svg = d3.select("#tree").append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    var treeLayout = d3.tree().size([height,width]);

    var rootNode = d3.hierarchy(root, function(d) { return d.children; });

    treeLayout(rootNode);

    //Create the links between nodes
    svg.selectAll('.link')
        .data(rootNode.links())
        .enter().append('path')
        .attr('class', 'link')
        .attr('d', d3.linkHorizontal().x(function(d) {return d.y; }).y(function(d) {return d.x}));

    //Create the nodes
    var node = svg.selectAll('.node')
        .data(rootNode.descendants())
        .enter().append('g')
        .attr('class','node')
        .attr('transform', function(d) { return 'translate(' + d.y + ',' + d.x + ')';});
        
    node.append('circle')
        .attr('r', 10)
        .style('fill', '#fff')  // Add fill color
        .style('stroke', '#000')  // Add stroke to make it visible
        .attr('class', function(d) { return d.children ? 'node--internal' : 'node--leaf'; });

        node.append('text')
            .attr('dy', '.35em')
            .attr('x', function(d) { return d.children ? -13 : 13; })
            .style('text-anchor', function(d) { return d.children ? 'end' : 'start'; })
            .style('fill', '#333')
            .text(function(d) { return d.data.name; });  
    }