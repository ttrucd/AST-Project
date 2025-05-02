//convert AST JSON into a tree
function jsontoTree(json) {
    var tree = {
        name: json.Case,
        children: (json.Fields || []).map(function(field) {
            if (typeof field == 'object') {
                return jsontoTree(field);
            }
            else{
                return { name:field };
            }
        })
    };
    return tree; 
}

function drawTree(astData) {
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

    var rootNode = d3.hierarchy(root, function(d) { return d.children;});
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
        .attr('class', function(d) { return d.children ? 'node--internal' : 'node--leaf'; });

    node.append('text')
        .attr('dy', '.35em')
        .attr('x', function(d) { return d.children ? -13 : 13; })
        .style('text-anchor', function(d) { return d.children ? 'end' : 'start'; })
        .text(function(d) { return d.data.name; });  
}