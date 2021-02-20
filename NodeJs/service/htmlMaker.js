var htmlMaker = {};
var _htmlStyle = "<style>";
    _htmlStyle += "table {width: 60%;border: 1px solid #444444;border-collapse: collapse;margin: auto;text-align: center;}";
    _htmlStyle +="th, td {border: 1px solid #444444;}tr {height:60px;}</style>";

htmlMaker.htmlMaker_Approve = 
    function htmlMaker_Approve (param) {
        var _html = _htmlStyle;
        _html += "<div style='text-align:center;font-size:x-large'>  ";
        _html += "<body>";
        _html += "<table>";
        _html += "<tbody>"

        _html += "<tr><td>프로젝트명</td><td>";
        _html += param.dept;
        _html += "</td></tr>";

        _html += "<tr><td>신청자</td><td>";
        _html += param.name;
        _html += "</td></tr>";

        _html += "<tr><td>근무일자</td><td>";
        _html += param.workDate;
        _html += "</td></tr>";

        _html += "<tr><td>출근시간</td><td>";
        _html += param.workTimeS;
        _html += "</td></tr>";

        _html += "<tr><td>퇴근시간</td><td>";
        _html += param.workTimeE;
        _html += "</td></tr>";

        _html += "<tr><td>야근시간</td><td>";
        _html += param.workOver;
        _html += "</td></tr>";

        _html += "<tr><td>야근사유</td><td>";
        _html += param.overTimeReason;
        _html += "</td></tr>";

        _html += "</tbody></table></body>";
        _html += "</div>"
        _html += "<br><br><br><br><br>"
        return _html;
    }

module.exports = htmlMaker;