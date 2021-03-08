/*
* TITLE  : 일정관리 관련 컨트롤
* AUTHOR : 김현수
* DATE   : 2021.01.25
* DESC   : 일정관리에 관련된 DAO서비스
*/

const express = require('express');
const session = require('express-session');
const utils = require('../utils/utils');

const schedule_add = (req, res) => {

    var database = req.app.get('database');

    var ScheduleInfos = new database.ScheduleModel({
        id:req.body.id,
        date:req.body.date,
        subject:req.body.subject,
        start:req.body.start,
        end:req.body.end,
        body:req.body.body
    });
    
    ScheduleInfos.saveSchedule(function(err, result) {
        if (err) {
            console.log(err);

            var param = {
                result : "NG"
            };
        }else{
            var param = {
                result : "OK"
            };

        }
        return res.send(param);
    });
}

const schedule_find = (req, res) => {

    var database = req.app.get('database');    
    var param = new Object();

    if(!utils.isEmpty(req.body.id)) {        
        param.id = req.body.id;
    }

    if(!utils.isEmpty(req.body.date)) {    
        param.date = req.body.date;
    }

    database.ScheduleModel.findSchedule(param, function(err, result) {
        var param = {
            scheduleList:result
        };

        res.send(param);
    });
}

const schedule_update = (req, res) => {

    var database = req.app.get('database');

    var setUpdateParam = new Object();
    setUpdateParam.subject = req.body.subject;
    setUpdateParam.date = req.body.date;
    setUpdateParam.start = req.body.start;
    setUpdateParam.end = req.body.end;
    setUpdateParam.body = req.body.body;

    database.ScheduleModel.updateSchedule({"_id":req.body._id}, setUpdateParam, function(err, result) {
        res.send(result);
    });    
}

const schedule_delete  = (req, res) => {

    var database = req.app.get('database');

    database.ScheduleModel.removeSchedule({"_id":req.body._id}, function(err, result) {
        res.send(result);
    });
}

module.exports = {
    schedule_add,
    schedule_find,
    schedule_update,
    schedule_delete
}