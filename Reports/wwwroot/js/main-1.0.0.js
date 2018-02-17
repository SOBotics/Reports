$(document).ready(function () {
	if (localStorage["bodyClass"] === "bodyLight") {
		$("body").addClass("bodyLight")
	}
	else {
		$("body").addClass("bodyDark")
	}

	sortReports()

	$(".switchToLight").click(function () {
		$("body").addClass("bodyLight").removeClass("bodyDark")
		localStorage["bodyClass"] = "bodyLight"
	});

	$(".switchToDark").click(function () {
		$("body").addClass("bodyDark").removeClass("bodyLight")
		localStorage["bodyClass"] = "bodyDark"
	});

	$("#openAllReports").click(function () {
		$(".reportLink a").each(function () {
			var url = $(this).attr("href")
			window.open(url)
		});
	});

	$(".timestamp").each(function() {
		var timestamp = new Date(+$(this)[0].dataset.unixtime)
		var secAge = (Date.now() - timestamp) / 1000
		$(this).attr("title", timestamp.toISOString().replace("T", " ").substr(0, 19) + "Z")
		if (secAge < 0 || secAge > 60 * 60 * 24 * 7 * 4) {
			var md = timestamp.toDateString().slice(4, 15)
			var y = "'" + md.split(" ")[2].slice(2, 4)
			var mdy = md.slice(0, 7) + y
			$(this).text(mdy)
			return
		} else if (secAge > 60 * 60 * 24 * 6) {
			var weeks = Math.round(secAge / 60 / 60 / 24 / 7)
			$(this).text(weeks + " week" + (weeks == 1 ? "" : "s"))
		} else if (secAge > 60 * 60 * 24) {
			var days = Math.round(secAge / 60 / 60 / 24)
			$(this).text(days + " day" + (days == 1 ? "" : "s"))
		} else if (secAge > 60 * 60) {
			var hours = Math.round(secAge / 60 / 60)
			$(this).text(hours + " hour" + (hours == 1 ? "" : "s"))
		} else if (secAge > 60) {
			var mins = Math.round(secAge / 60)
			$(this).text(mins + " minute" + (mins == 1 ? "" : "s"))
		}
		else {
			$(this).text("a few seconds")
		}
		$(this).text($(this).text() + " ago")
	})
})

$(window).load(applyReportLayout)

function sortReports() {
	if ($("#sortBy")[0] === undefined) return
	$("#sortBy").change(sortReports)
	$(".report").sort(function (a, b) {
		let sortByFieldId = $("#sortBy")[0].value
		let aEl = $("." + sortByFieldId + " .fieldData", a)
		let bEl = $("." + sortByFieldId + " .fieldData", b)
		let aData, bData = 0
		if (aEl[0].dataset.unixtime !== undefined) {
			aData = aEl[0].dataset.unixtime
			bData = bEl[0].dataset.unixtime
		} else {
			aData = aEl.text()
			bData = bEl.text()
		}
		return -(aData - bData)
	})
	.appendTo($("#main"))
}

function getFieldIdsByMaxWidth() {
	let fields = []
	$(".report div > span").each(function () {
		let width = $(this).outerWidth(true)
		let classes = [].slice.apply($(this).parent()[0].classList)
		let id = classes.filter(function (x) {
			return x.startsWith("FID")
		})[0]
		let f = fields.filter(function (x) {
			return x.id === id
		})[0]
		if (f === undefined) {
			fields.push({
				"id": id,
				"width": width,
			})
		} else if (f.width < width) {
			fields = fields.filter(function (x) {
				return x.id !== id
			})
			fields.push({
				"id": id,
				"width": width,
			})
		}
	})
	return fields.sort(function (a, b) {
		return a.width - b.width
	})
}

function getSmallFields(fields) {
	return fields.filter(function (x) {
		return fields[0].width + x.width < 900
	})
}

function getLargeFields(fields) {
	return fields.filter(function (x)
	{
		return fields[0].width + x.width >= 900
	})
}

function getsubCount(smallFields, largeFields) {
	let currentWidth = 0
	let subs = largeFields.length
	for (let i in smallFields) {
		let f = smallFields[i]
		if (currentWidth + f.width > 900) {
			subs++
			currentWidth = 0
		}
		currentWidth += f.width
	}
	if (currentWidth > 0) subs++
	return subs
}

function applyReportLayout() {
	if (!window.location.pathname.startsWith("/r")) return
	let fieldWidths = getFieldIdsByMaxWidth()
	let largeFields = getLargeFields(fieldWidths)
	let smallFields = getSmallFields(fieldWidths)
	let subCount = getsubCount(smallFields, largeFields)
	$("head").append("<style>.report div > span { margin-right: 0; }</style>")
	$(".report").each(function () {
		let currentWidth = 0
		let currentSub = 0
		for (let i = 0; i < subCount; i++) {
			$(this).append("<div class=\"reportSubcontainer\" />")
		}
		for (let i in smallFields) {
			let f = smallFields[i]
			if (currentWidth + f.width > 900) {
				currentWidth = 0
				currentSub++
			}
			currentWidth += f.width
			let fEl = $("." + f.id, this)
			$("> span", fEl).width(f.width)
			$(".reportSubcontainer:eq(" + currentSub + ")", this).append(fEl)
		}
		for (let i in largeFields) {
			let fId = largeFields[i].id
			let f = $("." + fId, this)
			$(".reportSubcontainer:empty:eq(0)", this).append(f)
		}
	})
}