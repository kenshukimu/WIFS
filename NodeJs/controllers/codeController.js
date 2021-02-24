/*
* TITLE  : 코드 컨트롤
* AUTHOR : 김현수
* DATE   : 2021.01.25
* DESC   : 코드테이블에 관련된 DAO서비스
*/

const express = require('express')

const code_Find = (req, res) => {
    var database = req.app.get('database');

    database.CodesModel.findCode({'codeGroup' : req.body.codeGroup}, function(err, result) {
        var param = {
            codes:result
        };
        res.send(param);
    });
}

const weekInfo_Find = (req, res) => {
    var database = req.app.get('database');
    
    database.WeekinfoModel.findWeekInfo({'year' : req.body.year}, function(err, result) {
        var param = {
            weekInfo:result
        };
        res.send(param);
    });
}

module.exports = {
    code_Find,
    weekInfo_Find
}