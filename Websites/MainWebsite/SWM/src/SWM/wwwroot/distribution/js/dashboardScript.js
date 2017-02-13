function user_dashboard(t){"use strict";function a(a,i){a.isBusyChart1=!0,a.isBusyChart5=!0,i.get("/api/"+t+"/location_info").then(function(t){for(var e=t.data,n=0;n<e.length;n++)u.push(e[n].name),g.push(e[n].totalWeight);u.length>1&&u.push("All"),a.locations=u,a.selectedLocation=u[u.length-1],a.displayChart1(),a.isBusyChart5=!1,r()},function(t){a.chart1Error="Unable to display chart"}),a.showMonthlyCharts=!0,a.chartClass="col-md-6",a.isBusyChart2=a.isBusyChart4=a.isBusyChart3=!0,i.get("/api/"+t+"/user_dates").then(function(r){if(h=r.data,h.length>1){for(var e=0;e<h.length;e++){var n=new Date(h[e]);c.push(f[n.getMonth()]+" "+n.getFullYear())}if(h.length>=p)var o=h.length-p;else var o=0;var l=h.length;a.monthLabels=c,a.chart2StartDate=c[o],a.chart2EndDate=c[l-1],i.get("/api/"+t+"/product_info").then(function(t){s(t.data),a.products=d,a.chart3ProductName=d[0],a.chart3StartDate=c[o],a.chart3EndDate=c[l-1],a.displayChart3()},function(t){a.chart3Error="Unable to display chart"}),a.chart4StartDate=c[o],a.chart4EndDate=c[l-1],a.displayChart2(),a.displayChart4()}else a.showMonthlyCharts=!1,a.chartClass="col-md-12"},function(t){a.chart2Error=a.chart3Error=a.chart4Error="Unable to display chart",a.isBusyChart2=a.isBusyChart3=a.isBusyChart4=!1}),a.displayChart1=function(){var r=[];if(a.isBusyChart1=!0,a.selectedLocation===u[u.length-1])i.get("/api/"+t+"/product_info").then(function(t){for(var e=t.data,n=0;n<e.length;n++)r.push(e[n]);a.isBusyChart1=!1,l(r)},function(t){a.chart1Error="Unable to display chart"});else for(var e=0;e<u.length;e++)u[e]==a.selectedLocation&&i.get("/api/"+t+"/"+u[e]+"/product_info").then(function(t){for(var e=t.data,n=0;n<e.length;n++)r.push(e[n]);a.isBusyChart1=!1,l(r)},function(t){a.chart1Error="Unable to display chart"})},a.displayChart2=function(){if(a.chart2StartDate==a.chart2EndDate)return a.chart2Error="Start month and end month must not have same value";for(var r,e,n=0;n<c.length;n++)c[n]===a.chart2StartDate?e=n:c[n]===a.chart2EndDate&&(r=n);if(e>r)return a.chart2Error="Start month must come before end month";a.isBusyChart2=!0;for(var l,s,n=0;n<h.length;n++)c[n]==a.chart2StartDate&&(l=new Date(h[n])),c[n]==a.chart2EndDate&&(s=new Date(h[n]));i.get("/api/"+t+"/product_info_month_wise/"+(l.getMonth()+1)+"/"+l.getFullYear()+"/"+(s.getMonth()+1)+"/"+s.getFullYear()).then(function(t){a.isBusyChart2=!1,a.chart2Error=o(a.chart2StartDate,a.chart2EndDate,t.data)},function(t){a.isBusyChart2=!1,a.chart2Error="Unable to display chart"})},a.displayChart3=function(){if(a.chart3StartDate==a.chart3EndDate)return a.chart3Error="Start month and end month must not have same value";for(var r,e,o=0;o<c.length;o++)c[o]===a.chart3StartDate?e=o:c[o]===a.chart3EndDate&&(r=o);if(e>r)return a.chart3Error="Start month must come before end month";a.isBusyChart3=!0;for(var l,s,o=0;o<h.length;o++)c[o]==a.chart3StartDate&&(l=new Date(h[o])),c[o]==a.chart3EndDate&&(s=new Date(h[o]));i.get("/api/"+t+"/product_info_month_wise/"+(l.getMonth()+1)+"/"+l.getFullYear()+"/"+(s.getMonth()+1)+"/"+s.getFullYear()).then(function(t){a.isBusyChart3=!1,a.chart3Error=n(a.chart3ProductName,a.chart3StartDate,a.chart3EndDate,t.data)},function(t){a.isBusyChart3=!1,a.chart3Error="Unable to display chart"})},a.displayChart4=function(){if(a.chart2StartDate==a.chart2EndDate)return a.chart4Error="Start month and end month must not have same value";for(var r,n,o=0;o<c.length;o++)c[o]===a.chart2StartDate?n=o:c[o]===a.chart2EndDate&&(r=o);if(n>r)return a.chart4Error="Start month must come before end month";a.isBusyChart4=!0;for(var l,s,o=0;o<h.length;o++)c[o]==a.chart4StartDate&&(l=new Date(h[o])),c[o]==a.chart4EndDate&&(s=new Date(h[o]));i.get("/api/"+t+"/product_info_month_wise/"+(l.getMonth()+1)+"/"+l.getFullYear()+"/"+(s.getMonth()+1)+"/"+s.getFullYear()).then(function(t){a.isBusyChart4=!1,a.chart4Error=e(a.chart4StartDate,a.chart4EndDate,t.data)},function(t){a.isBusyChart4=!1,a.chart4Error="Unable to display chart"})}}function r(){for(var t=[],a=0;a<u.length-1;a++)t.push(u[a]);var r={labels:t,datasets:[{data:g,backgroundColor:"#ff7272"}]},e={maintainAspectRatio:!1,scales:{xAxes:[{display:!0,scaleLabel:{display:!0,labelString:"Weight"},ticks:{beginAtZero:!0}}],yAxes:[{display:!0,scaleLabel:{display:!0,labelString:"Locations"}}]},legend:{display:!1},tooltips:{enabled:!0,mode:"single",callbacks:{label:function(t,a){return t.xLabel<=1e3?t.xLabel+" gm":t.xLabel/1e3+" kg"}},displayColors:!1}},n=$("#locations").height()-Math.floor(.3*$("#locations").height());$("#chart5div").empty(),$("#chart5div").append('<canvas id="chart5" style="height:'+n+'" height="'+n+'" width="787"></canvas>');var o=document.getElementById("chart5").getContext("2d");new Chart(o,{type:"horizontalBar",data:r,options:e})}function e(t,a,r){function e(t){if(Array.isArray(t)){for(var a=0,r=Array(t.length);a<t.length;a++)r[a]=t[a];return r}return Array.from(t)}for(var n,o,l=0;l<c.length;l++)c[l]===t?o=l:c[l]===a&&(n=l);for(var i=function(t){return[].concat(e(new Set(t)))},s=function(t){return[].concat.apply([],t)},h=r.map(function(t){return t.date}),d=i(s(r.map(function(t){return t.productInformation})).map(function(t){return t.productName})),u=d.map(function(t){return{data:h.map(function(a){var e=r.find(function(t){return t.date===a}).productInformation.find(function(a){return a.productName===t});return e?e.totalWeight:0})}}),p=[],l=o;l<=n;l++)p.push(c[l]);for(var g=[],l=0;l<u[0].data.length;l++){for(var f=0,y=0;y<u.length;y++)f+=u[y].data[l];g.push(f)}var m={labels:p,datasets:[{label:"Weight in grams",data:g,backgroundColor:"#ff7272"}]},b={maintainAspectRatio:!1,scales:{xAxes:[{display:!0,scaleLabel:{display:!0,labelString:"Months"},ticks:{beginAtZero:!0}}],yAxes:[{display:!0,scaleLabel:{display:!0,labelString:"Weight In Grams"}}]},legend:{display:!1},tooltips:{enabled:!0,mode:"single",callbacks:{label:function(t,a){return t.xLabel<=1e3?t.xLabel+" gm":t.xLabel/1e3+" kg"}},displayColors:!1}},v=0;v=r.length>15?250+Math.floor(100*r.length*.09):250,$("#chart4div").empty(),$("#chart4div").append('<canvas id="chart4" style="height: '+v+'px;" height="'+v+'" width="787"></canvas>');var C=document.getElementById("chart4").getContext("2d");new Chart(C,{type:"horizontalBar",data:m,options:b});return""}function n(t,a,r,e){for(var n,o,l=0;l<c.length;l++)c[l]===a?o=l:c[l]===r&&(n=l);for(var i=[],l=o;l<=n;l++)i.push(c[l]);for(var s,h=[],l=0;l<e.length;l++){for(s=0;s<e[l].productInformation.length;s++)if(e[l].productInformation[s].productName==t){h.push(e[l].productInformation[s].totalWeight);break}s==e[l].productInformation.length&&h.push(0)}var d=[{label:t,data:h,backgroundColor:"#f7ab40",borderWidth:5,borderColor:"#d68413",cubicInterpolationMode:"monotone"}],u={labels:i,datasets:d},p={maintainAspectRatio:!1,scales:{xAxes:[{display:!0,scaleLabel:{display:!0,labelString:"Products"}}],yAxes:[{display:!0,scaleLabel:{display:!0,labelString:"Weight In Grams"},ticks:{beginAtZero:!0}}]},legend:{display:!1,labels:{display:!1}},tooltips:{enabled:!0,mode:"single",callbacks:{label:function(t,a){return t.yLabel<=1e3?t.yLabel+" gm":t.yLabel/1e3+" kg"}},displayColors:!1}};$("#chart3div").empty(),$("#chart3div").append('<canvas id="chart3" style="height:250px;" height="250"></canvas>');var g=document.getElementById("chart3").getContext("2d");new Chart(g,{type:"line",data:u,options:p});return""}function o(t,a,r){function e(t){if(Array.isArray(t)){for(var a=0,r=Array(t.length);a<t.length;a++)r[a]=t[a];return r}return Array.from(t)}for(var n,o,l=0;l<c.length;l++)c[l]===t?o=l:c[l]===a&&(n=l);for(var s=function(t){return[].concat(e(new Set(t)))},h=function(t){return[].concat.apply([],t)},d=r.map(function(t){return t.date}),u=s(h(r.map(function(t){return t.productInformation})).map(function(t){return t.productName})),p=u.map(function(t){return{label:t,data:d.map(function(a){var e=r.find(function(t){return t.date===a}).productInformation.find(function(a){return a.productName===t});return e?e.totalWeight:0}),backgroundColor:i()}}),g=[],l=o;l<=n;l++)g.push(c[l]);var f={labels:g,datasets:p},y={maintainAspectRatio:!1,responsive:!0,legend:{display:!0,position:"top"},scales:{xAxes:[{display:!0,stacked:!0,scaleLabel:{display:!0,labelString:"Months"}}],yAxes:[{display:!0,stacked:!0,scaleLabel:{display:!0,labelString:"Weight In Grams"}}]},tooltips:{enabled:!0,mode:"single",callbacks:{label:function(t,a){return t.yLabel<=1e3?a.datasets[t.datasetIndex].label+": "+t.yLabel+" gm":a.datasets[t.datasetIndex].label+": "+t.yLabel/1e3+" kg"}}}};$("#chart2div").empty(),$("#chart2div").append('<canvas id="chart2" style="height:300px;" height="300"></canvas>');var m=document.getElementById("chart2").getContext("2d");new Chart(m,{type:"bar",data:f,options:y});return""}function l(t){for(var a=[],r=[],e=0;e<t.length;e++)a.push(t[e].productName),r.push(t[e].totalWeight);var n={labels:a,datasets:[{label:"Weight in grams",data:r,backgroundColor:"#3c8dbc"}]},o={maintainAspectRatio:!1,scales:{xAxes:[{display:!0,scaleLabel:{display:!0,labelString:"Products"}}],yAxes:[{display:!0,scaleLabel:{display:!0,labelString:"Weight In Grams"},ticks:{beginAtZero:!0}}]},legend:{display:!1},tooltips:{enabled:!0,mode:"single",callbacks:{label:function(t,a){var r=t.yLabel;return r<=1e3?t.yLabel+" gm":t.yLabel/1e3+" kg"}},displayColors:!1}};$("#chart1div").empty(),$("#chart1div").append('<canvas id="chart1" style="height: 250px;" height="250" width="787"></canvas>');var l=document.getElementById("chart1").getContext("2d");new Chart(l,{type:"bar",data:n,options:o})}function i(){for(var t="0123456789ABCDEF",a="#",r=0;r<6;r++)a+=t[Math.floor(16*Math.random())];return a}function s(t){for(var a=0;a<t.length;a++)d.push(t[a].productName)}var h=[],c=[],d=[],u=[],p=8,g=[],f=["January","February","March","April","May","June","July","August","September","October","November","December"];angular.module("app-dashboard",[]),angular.module("app-dashboard").controller("chartsController",["$scope","$http",a])}