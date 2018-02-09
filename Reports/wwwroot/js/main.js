$(document).ready(function () {
	if (localStorage["bodyClass"] === "bodyLight") {
		$("body").addClass("bodyLight")
	}
	else {
		$("body").addClass("bodyDark")
	}
	sortReports()
	$("#sortBy").change(sortReports)
	$(".switchToLight").click(function () {
		$("body").addClass("bodyLight").removeClass("bodyDark")
		localStorage["bodyClass"] = "bodyLight"
	});
	$(".switchToDark").click(function () {
		$("body").addClass("bodyDark").removeClass("bodyLight")
		localStorage["bodyClass"] = "bodyDark"
	});
	$("#openAllReports").click(function() {
		$(".fieldInner a").each(function() {
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

function sortReports() {
	$(".report").sort(function (a, b) {
		let sortByFieldId = $("#sortBy")[0].value
		let aEl = $("#" + sortByFieldId + " .fieldData", a)
		let bEl = $("#" + sortByFieldId + " .fieldData", b)
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

$(window).load(function () {
	fillTables()
})

function getCellIdsByMaxWidth() {
	let cells = []
	$(".report div:not(.reportLink)").each(function () {
		let width = $("> :first-child", $(this)).outerWidth(true)
		let id = $(this)[0].id
		let c = cells.filter(function (x) {
			return x.id === id
		})[0]
		if (c === undefined) {
			cells.push({
				"id": id,
				"width": width
			})
		} else if (c.width < width) {
			cells = cells.filter(function (x) {
				return x.id !== id
			})
			cells.push({
				"id": id,
				"width": width
			})
		}
	})
	return cells.sort(function (a, b) {
		return a.width - b.width
	})
}

function getSmallCells(cells) {
	return cells.filter(function (x) {
		return cells[0].width + x.width < 900
	})
}

function getLargeCells(cells) {
	return cells.filter(function (x)
	{
		return cells[0].width + x.width >= 900
	})
}

function getTableLayout(smallCells, rows) {
	let cells = JSON.parse(JSON.stringify(smallCells))
	cells.reverse()
	let tableLayout = []
	for (let i = 0; i < rows; i++) {
		tableLayout.push([])
	}
	for (let i = 0; i < cells.length; i++) {
		let rowIndex = i % rows
		tableLayout[rowIndex].push(cells[i])
	}
	return tableLayout
}

function getRowCount(smallCells, largeCells) {
	let smallCellsSorted = JSON.parse(JSON.stringify(smallCells))
	let flip = false
	for (let i = 0; i < smallCellsSorted.length; i++) {
		if (flip && i < smallCellsSorted.length / 2) {
			let temp = smallCellsSorted[i]
			smallCellsSorted[i] = smallCellsSorted[smallCellsSorted.length - i]
			smallCellsSorted[smallCellsSorted.length - i] = temp
		}
		flip = !flip
	}
	let rows = largeCells.length
	let currentRowWidth = 0
	for (let i = 0; i < smallCellsSorted.length; i++) {
		if (currentRowWidth + smallCellsSorted[i].width >= 900) {
			rows++
			currentRowWidth = 0
		}
		currentRowWidth += smallCellsSorted[i].width
	}
	if (currentRowWidth > 0) {
		rows++
	}
	return rows
}

function getColumnCount(tableLayout) {
	let columns = 0
	for (var rowLayoutIndex in tableLayout) {
		if (tableLayout[rowLayoutIndex].length > columns) {
			columns = tableLayout[rowLayoutIndex].length
		}
	}
	return columns
}

function fillTables() {
	let cellIdsByMaxWidth = getCellIdsByMaxWidth()
	let largeCells = getLargeCells(cellIdsByMaxWidth)
	let smallCells = getSmallCells(cellIdsByMaxWidth)
	let rows = getRowCount(smallCells, largeCells)
	let tableLayout = getTableLayout(smallCells, rows - largeCells.length)
	let columns = getColumnCount(tableLayout)
	$(".report").each(function () {
		for (let i = 0; i < rows; i++) {
			$("tbody", this).append("<tr></tr>")
		}
		for (let rowIndex in tableLayout) {
			let rowLayout = tableLayout[rowIndex]
			for (let cellIndex in rowLayout) {
				let cellID = rowLayout[cellIndex].id
				let cell = $("#" + cellID, this)
				let cellWidth = cellIdsByMaxWidth.filter(function (x) {
					return x.id === cellID
				})[0].width
				cell.width(cellWidth)
				cell = $("<td></td>").append(cell)
				$("tbody tr:eq(" + rowIndex + ")", this).append(cell)
			}
		}
		for (let cellIndex in largeCells) {
			let cellId = largeCells[cellIndex].id
			let cell = $("#" + cellId, this)
			cell = $("<td colspan=" + columns + "></td>").append(cell)
			$("tr:empty:eq(0)", this).append(cell)
		}
	})
}