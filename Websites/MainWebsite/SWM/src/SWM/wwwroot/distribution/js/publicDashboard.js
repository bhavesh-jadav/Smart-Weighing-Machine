!function(){"use strict";function t(t,h,u){var d=u.get("/api/public_dashboard/",{catchError:!0,delay:1e4});d.promise.then(null,null,function(a){null!=a.data?(g(a.data),t.gettingUserDetails=!1):($("#errorMessage").append('<div class="alert alert-danger alert-dismissible"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>There was a problem while getting latest data. Make sure you are connected to the internet and Make sure that it is working fine.</div>'),u.stopAll())});var g=function(a){var e=0;a.totalWeight>=1e6?(e=a.totalWeight/1e6,t.unit="Tonne(s)"):a.totalWeight>=1e3?(e=a.totalWeight/1e3,t.unit="KG(s)"):(e=a.totalWeight,t.unit="Gram(s)"),setTimeout(function(){$("#totalWeight").text(e),$("#totalProducts").text(a.totalProducts),$("#totalUsers").text(a.totalUsers),$("#totalMachines").text(a.totalMachines),$("#totalUserLocations").text(a.totalUserLocations),$("#lastUserRegisterd").text()!=a.lastUserRegisterd&&($("#lastUserRegisterd").fadeToggle(function(){$("#lastUserRegisterd").text(a.lastUserRegisterd)}),$("#lastUserRegisterd").fadeToggle())},10)};t.isBusyChart2=!0,t.isBusyChart1=!0,h.get("/api/user_dates").then(function(a){n=a.data;for(var e=0;e<n.length;e++){var u=new Date(n[e]);o.push(c[u.getMonth()]+" "+u.getFullYear())}if(t.monthLabels=o,n.length>=s)var d=n.length-s;else var d=0;var g=n.length;if(t.chart1StartDate=o[d],t.chart1EndDate=o[g-1],n.length>=i)var d=n.length-i;else var d=0;var g=n.length;t.chart2StartDate=o[d],t.chart2EndDate=o[g-1],h.get("/api/product_info").then(function(a){r(a.data),t.products=l,t.chart2ProductName=l[0],t.displayChart2()},function(a){t.chart2Error="Unable to display chart",t.isBusyChart2=!1}),t.displayChart1()},function(a){t.chart1Error="Unable to display chart",t.isBusyChart1=t.isBusyChart2=!1}),t.displayChart1=function(){if(t.chart1StartDate==t.chart1EndDate)return t.chart1Error="Start month and end month must not have same value";for(var e,r,l=0;l<o.length;l++)o[l]===t.chart1StartDate?r=l:o[l]===t.chart1EndDate&&(e=l);if(r>e)return t.chart1Error="Start month must come before end month";t.isBusyChart1=!0;for(var i,s,l=0;l<n.length;l++)o[l]==t.chart1StartDate&&(i=new Date(n[l])),o[l]==t.chart1EndDate&&(s=new Date(n[l]));h.get("/api/product_info_month_wise/"+(i.getMonth()+1)+"/"+i.getFullYear()+"/"+(s.getMonth()+1)+"/"+s.getFullYear()).then(function(e){t.isBusyChart1=!1,t.chart1Error=a(t.chart1StartDate,t.chart1EndDate,e.data)},function(a){t.isBusyChart1=!1,t.chart1Error="Unable to display chart"})},t.displayChart2=function(){if(t.chart2StartDate==t.chart2EndDate)return t.chart2Error="Start month and end month must not have same value";for(var a,r,l=0;l<o.length;l++)o[l]===t.chart2StartDate?r=l:o[l]===t.chart2EndDate&&(a=l);if(r>a)return t.chart2Error="Start month must come before end month";t.isBusyChart2=!0;for(var i,s,l=0;l<n.length;l++)o[l]==t.chart2StartDate&&(i=new Date(n[l])),o[l]==t.chart2EndDate&&(s=new Date(n[l]));h.get("/api/product_info_month_wise/"+(i.getMonth()+1)+"/"+i.getFullYear()+"/"+(s.getMonth()+1)+"/"+s.getFullYear()).then(function(a){t.isBusyChart2=!1,t.chart2Error=e(t.chart2ProductName,t.chart2StartDate,t.chart2EndDate,a.data)},function(a){t.isBusyChart2=!1,t.chart2Error="Unable to display chart"})}}function a(t,a,e){function r(t){if(Array.isArray(t)){for(var a=0,e=Array(t.length);a<t.length;a++)e[a]=t[a];return e}return Array.from(t)}for(var n,l,i=0;i<o.length;i++)o[i]===t?l=i:o[i]===a&&(n=i);for(var s=function(t){return[].concat(r(new Set(t)))},c=function(t){return[].concat.apply([],t)},h=e.map(function(t){return t.date}),u=s(c(e.map(function(t){return t.productInformation})).map(function(t){return t.productName})),d=u.map(function(t){return{data:h.map(function(a){var r=e.find(function(t){return t.date===a}).productInformation.find(function(a){return a.productName===t});return r?r.totalWeight:0})}}),g=[],i=l;i<=n;i++)g.push(o[i]);for(var p=[],i=0;i<d[0].data.length;i++){for(var f=0,m=0;m<d.length;m++)f+=d[m].data[i];p.push(f)}var y={labels:g,datasets:[{label:"Weight in grams",data:p,backgroundColor:"#ff7272"}]},b={maintainAspectRatio:!1,scales:{xAxes:[{display:!0,scaleLabel:{display:!0,labelString:"Months"}}],yAxes:[{display:!0,scaleLabel:{display:!0,labelString:"Weight In Grams"},ticks:{beginAtZero:!0}}]},legend:{display:!1},tooltips:{enabled:!0,mode:"single",callbacks:{label:function(t,a){return t.yLabel<=1e3?t.yLabel+" gm":t.yLabel/1e3+" kg"}},displayColors:!1}};$("#chart1div").empty(),$("#chart1div").append('<canvas id="chart1" style="height:300px;" height="300"></canvas>');var v=document.getElementById("chart1").getContext("2d");new Chart(v,{type:"bar",data:y,options:b});return""}function e(t,a,e,r){for(var n,l,i=0;i<o.length;i++)o[i]===a?l=i:o[i]===e&&(n=i);for(var s=[],i=l;i<=n;i++)s.push(o[i]);for(var c,h=[],i=0;i<r.length;i++){for(c=0;c<r[i].productInformation.length;c++)if(r[i].productInformation[c].productName==t){h.push(r[i].productInformation[c].totalWeight);break}c==r[i].productInformation.length&&h.push(0)}var u=[{label:t,data:h,backgroundColor:"#f7ab40",borderWidth:5,borderColor:"#d68413",cubicInterpolationMode:"monotone"}],d={labels:s,datasets:u},g={maintainAspectRatio:!1,scales:{xAxes:[{display:!0,scaleLabel:{display:!0,labelString:"Products"}}],yAxes:[{display:!0,scaleLabel:{display:!0,labelString:"Weight In Grams"},ticks:{beginAtZero:!0}}]},legend:{display:!1,labels:{display:!1}},tooltips:{enabled:!0,mode:"single",callbacks:{label:function(t,a){return t.yLabel<=1e3?t.yLabel+" gm":t.yLabel/1e3+" kg"}},displayColors:!1}};$("#chart2div").empty(),$("#chart2div").append('<canvas id="chart2" style="height:250px;" height="250"></canvas>');var p=document.getElementById("chart2").getContext("2d");new Chart(p,{type:"line",data:d,options:g});return""}function r(t){for(var a=0;a<t.length;a++)l.push(t[a].productName)}var n=[],o=[],l=[],i=10,s=15,c=["January","February","March","April","May","June","July","August","September","October","November","December"];angular.module("public",["emguo.poller"]),angular.module("public").controller("chartsController",["$scope","$http","poller",t])}();