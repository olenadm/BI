// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function numberToString(num){
    return num.toLocaleString(undefined, {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2}); 
}

let currentColumn = -1;
let isAscending = true;
function sortTable(table, columnIndex, isNumeric) {
    console.log(table);
    const rows = Array.from(table.rows).slice(1); // Ignore header row


    if (currentColumn === columnIndex) {
        isAscending = !isAscending;
    } else {
        currentColumn = columnIndex;
        isAscending = true;
    }

    let cellA;
    let cellB;
    rows.sort((a, b) => {
        if(isNumeric == 'true') {
            cellA = a.cells[columnIndex].textContent
                        .replace(".", "")
                        .replace(',', '')
                        .trim()
                        .padStart(20,'0');
            cellB = b.cells[columnIndex].textContent
                        .replace(".", "")
                        .replace(',', '')
                        .trim()
                        .padStart(20,'0');
            return isNaN(cellA) ? cellA.localeCompare(cellB) : cellA - cellB;
        } else 
        {
            cellA = a.cells[columnIndex].textContent.trim();
            cellB = b.cells[columnIndex].textContent.trim();
            return isNaN(cellA) ? cellA.localeCompare(cellB) : cellB.localeCompare(cellA);
        }
    });

    if (!isAscending) {
        rows.reverse();
    }

    while (table.rows.length > 1) {
        table.deleteRow(1);
    }

    rows.forEach(row => {
        table.appendChild(row);
    });

    //não implementado a troca de ícone de sinalização da ordenação
    //@* updateSortIcons(columnIndex, cod);             *@
}

function updateSortIcons(columnIndex, cod) {
    const tableHeaders = document.querySelectorAll("#prodComprasFornecedores"+cod);
    tableHeaders.forEach((th, index) => {
        const icon = th.querySelector(".sort-icon");
        if (index === columnIndex) {
            icon.innerHTML = isAscending ? "&#x2191;" : "&#x2193;";
        } else {
            icon.innerHTML = "&#x2195;";
        }
    });
}
