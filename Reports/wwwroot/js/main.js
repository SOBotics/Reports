$(document).ready(function () {
	if (localStorage["bodyClass"] === "bodyLight") {
		$("body").addClass("bodyLight");
	}
	else {
		$("body").addClass("bodyDark");
	}
	prepareLayout();
	$(".switchToLight").click(function () {
		$("body").addClass("bodyLight").removeClass("bodyDark");
		localStorage["bodyClass"] = "bodyLight";
	});
	$(".switchToDark").click(function () {
		$("body").addClass("bodyDark").removeClass("bodyLight");
		localStorage["bodyClass"] = "bodyDark";
	});
	$("#openAllReports").click(function() {
		$(".fieldInner a").each(function() {
			var url = $(this).attr("href");
			window.open(url);
		});
	});
	$(".timestamp").each(function() {
		var timestamp = new Date(+$(this)[0].dataset.unixtime * 1000);
		var secAge = (Date.now() - timestamp) / 1000;

		$(this).attr("title", timestamp.toISOString().replace("T", " ").substr(0, 19) + "Z");

		if (secAge < 5) {
			$(this).text("a few seconds");
		}
		else if (secAge < 59) {
			var secs = Math.round(secAge);
			$(this).text(secs + " second" + (secs == 1 ? "" : "s"));
		}
		else if (secAge < 60 * 59) {
			var mins = Math.round(secAge / 60);
			$(this).text(mins + " minute" + (mins == 1 ? "" : "s"));
		}
		else if (secAge < 60 * 60 * 23) {
			var hours = Math.round(secAge / 60 / 60);
			$(this).text(hours + " hour" + (hours == 1 ? "" : "s"));
		}
		else if (secAge < 60 * 60 * 24 * 6) {
			var days = Math.round(secAge / 60 / 60 / 24);
			$(this).text(days + " day" + (days == 1 ? "" : "s"));
		}
		else if (secAge < 60 * 60 * 24 * 7 * 4) {
			var weeks = Math.round(secAge / 60 / 60 / 24 / 7);
			$(this).text(weeks + " week" + (weeks == 1 ? "" : "s"));
		}
		else {
			var md = timestamp.toDateString().slice(4, 15);
			var y = "'" + md.split(" ")[2].slice(2, 4)
			var mdy = md.slice(0, 7) + y;
			$(this).text("on " + mdy);
			return;
		}
		$(this).text($(this).text() + " ago");
	});
});

function prepareLayout() {
	let fieldIdsByWidth = {};
	let aID = null;
	$(".fieldOuter").each(function () {
		let id = $(this)[0].id;
		let child = $("> :first-child", $(this));
		let width = child.outerWidth(true) + 1;

		if ($("> :first-child", $(child)).prop("tagName") === "A") {
			aID = id;
		} else if (fieldIdsByWidth[id] === undefined || fieldIdsByWidth[id] < width) {
			fieldIdsByWidth[id] = width;
		}
	});
	let css = "<style> .fieldInner { margin: 0; }";
	for (let id in fieldIdsByWidth) {
		css += "\n\r#" + id + "{ width: " + fieldIdsByWidth[id] + "px }";
	}
	$("head").append(css + "\n\r</style>");
	let subs = (aID !== null ? 1 : 0);
	let currentWidth = 0;
	for (let id in fieldIdsByWidth) {
		if (currentWidth + fieldIdsByWidth[id] >= 900) {
			subs++;
			currentWidth = 0;
		}
		currentWidth += fieldIdsByWidth[id];
	}
	if (currentWidth > 0) {
		subs++;
	}
	$(".report").each(function () {
		for (let i = 0; i < subs; i++) {
			$(this).append("<div class=\"reportSubcontainer\"></div>");
		}
	});
	if (aID !== null) {
		$(".report").each(function () {
			let newParent = $(".reportSubcontainer:first", $(this));
			let f = $("span a", $(this)).parent().parent().appendTo(newParent);
		});
	}
	$(".report").each(function () {
		let fields = $("> .fieldOuter", $(this));
		fields = fields.sort(function (a, b) {
			let aWidth = fieldIdsByWidth[$(a)[0].id];
			let bWidth = fieldIdsByWidth[$(b)[0].id];
			return aWidth - bWidth;
		});
		let currentSub = aID !== null ? 1 : 0;
		let currentWidth = 0;
		for (let i = 0; i < fields.length; i++) {
			let w = fieldIdsByWidth[$(fields[i])[0].id];
			if (currentWidth + w >= 900) {
				currentSub++;
				currentWidth = 0;
			}
			currentWidth += w;
			$(".reportSubcontainer", $(this))[currentSub].append(fields[i]);
		}
	});
}