$(document).ready(function() {
	$(".CurrentYear").text(new Date().getFullYear())
});

$.ajax({
    url: "https://raw.githubusercontent.com/whitestone-no/open-serial-port-monitor/master/README.md",
    dataType: 'text',
    success: function(data) {
 
        // Convert readme from markdown to html
        //var converter = new Markdown.Converter();
 
        // Show html
        $("#ReadMe").html(marked(data));
 
    }
});