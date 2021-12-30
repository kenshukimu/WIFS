/*
* TITLE  : 근무시간 관련 컨트롤
* AUTHOR : 김현수
* DATE   : 2021.01.25
* DESC   : 근무시간에 관련된 DAO서비스
*/

//const express = require('express');
const session = require('express-session');
const sendMail = require('../service/sendMail');
const htmlmaker = require('../service/htmlMaker');
const utils = require('../utils/utils');

const logger = require('../config/winston')('server');

const init_workInfo = (req, res) => {
    //로그인 세션확인처리
    if(utils.isEmpty(req.session.userId)) {
        res.render('index' , {message:''});     
    }else{
        res.render('workInfo', {role:req.session.role, userId:req.session.userId, projectNm:req.session.dept});  
    }
}

const init_workUsers = (req, res) => {   
    //로그인 세션확인처리
    if(utils.isEmpty(req.session.userId)) {
        res.render('index' , {message:''});     
    }else{
        res.render('workUsers', {role:req.session.role, userId:req.session.userId, projectNm:req.session.dept});  
    }   
}

const init_workVac = (req, res) => {   
    //로그인 세션확인처리
    if(utils.isEmpty(req.session.userId)) {
        res.render('index' , {message:''});     
    }else{
        res.render('workVac', {role:req.session.role, userId:req.session.userId, projectNm:req.session.dept});  
    }   
}

const move_popupWorkInfo = (req, res) => {   
    //로그인 세션확인처리
    if(utils.isEmpty(req.session.userId)) {
        res.render('index' , {message:''});     
    }else{
        var _kbName;
        if(req.query.kb == '0') {
            _kbName= "휴가";
        }else{
            _kbName= "근무";
        }
        res.render('popupWorkInfo' , {_id:req.query._id, _role:req.session.role, _name:req.query.name, kbName:_kbName, _kb:req.query.kb});     
    }   
}

const workinfo_add = (req, res) => {

    var database = req.app.get('database');

    var WorkInfos = new database.WorkinfoModel({
        id:req.body.id,
        workDate:req.body.workDate,
        workTimeS:req.body.workTimeS,
        workTimeE:req.body.workTimeE,
        workHour:req.body.workHour,
        workOver:req.body.workOver,
        dinnerTime:req.body.dinnerTime,
        lunchTime:req.body.lunchTime,
        status:req.body.status,
        overTimeReason:req.body.overTimeReason
    });
    
    WorkInfos.saveWorkInfo(function(err, result) {
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

const workinfo_find = (req, res) => {

    var database = req.app.get('database');
    
    var param = new Object();

    if(!utils.isEmpty(req.body.id)) {
        param.id = req.body.id;
    }    

    if(!utils.isEmpty(req.body.searchDate)) {
        
        var _date = req.body.searchDate.split("^");

        var _dateCriteria = new Object();
        _dateCriteria.$gte = _date[0];
        _dateCriteria.$lte = _date[1];

        param.workDate = _dateCriteria;
    }

    if(!utils.isEmpty(req.body.kb)) {
        var _hourCriteria = new Object();

        if(req.body.kb == '0') {            
            _hourCriteria.$gt = 0;
            param.workHour = _hourCriteria;
        }else{
            _hourCriteria.$lt = 0;
            param.workOver = _hourCriteria;
        }        
    }

    if(!utils.isEmpty(req.body._id)) {
        param._id = req.body._id;
    }   


    database.WorkinfoModel.findWorkInfo(param, function(err, result) {
        var param = {
            workList:result
        };

        res.send(param);
    });
}

const workInfo_update = (req, res) => {

    var database = req.app.get('database');
    var _htmlparam = {
        name : req.body.name,
        workDate : req.body.workDate,
        workTimeS : req.body.workTimeS,
        workTimeE : req.body.workTimeE,
        workOver : req.body.workOver,
        overTimeReason : req.body.overTimeReason,
        dept : req.session.dept
    };

    var _html = htmlmaker.htmlMaker_Approve(_htmlparam, req.body._kb);
    
    database.WorkinfoModel.updateWorkInfo({"_id":req.body._id}, {"status":req.body._status}, function(err, result) {
        res.send(result);
    });    

    //승인이 완료 후 특정인에게 메일보내기
    if(req.body._status == '1') {
        if(req.body._kb == '1') {
            var param = {
                toMail : 'shk1403@kico.co.kr;',
                subJect : req.session.dept + ' ' + req.body.name + ' 야근 신청' ,
                html : _html + '<div style="font: bold italic 2.0em/1.0em 돋움체;"> 위와 같이 야근을 신청합니다.<div>'
            };
        }else{
            var param = {
                toMail : 'shk1403@kico.co.kr;hsookim@kico.co.kr;',
                subJect : req.session.dept + ' ' + req.body.name + ' 대체휴가 신청' ,
                html : _html + '<div style="font: bold italic 2.0em/1.0em 돋움체;"> 위와 같이 대체휴가를 신청합니다.<div>'
            };
        }
        sendMail.sendMailByGmail(param);
    }   
}

const workInfo_delete  = (req, res) => {

    var database = req.app.get('database');
    
    database.WorkinfoModel.removeWorkInfo({"_id":req.body._id}, function(err, result) {
        res.send(result);
    });
}

const workInfo_updateAll = (req, res) => {

    var database = req.app.get('database');
   
    var _body = "";
    var param = new Object();
    param.departNo = req.session.deptNo;

    var sDate = '20000101';
    var eDate = '20300101';

    database.UserModel.findUserWithWorkList(param, "0", sDate, eDate, function(err, result) {
        var i = 0;
        var _result;
        result.forEach(function(doc,err) {
            doc.workinfoList.forEach(function(wifl) {
                _body +="<tr><td>";
                var _tmp = "야근신청";
                if(wifl.workOver < 0) {
                    _tmp = "휴가신청";
                }
                _body +="</td>";
                _body += _tmp;
                _body +="<td>";
                _body += doc.name;
                _body +="</td>";
                _body +="<td>";
                _body += wifl.workDate.substring(0,4)+"년 "+ wifl.workDate.substring(4,6) +"월 "+  wifl.workDate.substring(6,8) +"일 ";

                _body +="</td>";
                _body +="<td>";
                _body += wifl.workTimeS.substring(8,10)+ "시 "+ wifl.workTimeS.substring(10,12)+ "분";
                _body +="</td>";
                _body +="<td>";
                _body += wifl.workTimeE.substring(8,10)+ "시 "+ wifl.workTimeE.substring(10,12)+ "분";
                _body +="</td>";
                _body +="<td>";
                _body += wifl.workHour;
                _body +="분</td>";
                _body +="<td>";
                _body += wifl.overTimeReason;
                _body +="</td></tr>";
                database.WorkinfoModel.updateWorkInfo({"_id":wifl._id}, {"status":"1"}, function(err, result) {
                    _result = result;
                });  
            });            
        });

        var _html = htmlmaker.htmlMaker_Approve_All(_body);
    
        //승인이 완료 후 특정인에게 메일보내기
        var param = {
            toMail : 'shk1403@kico.co.kr;',
            subJect : req.session.dept + ' 초과근무 및 대체휴가 일괄 신청' ,
            html : _html + '<div style="font: bold italic 2.0em/1.0em 돋움체;"> 위와 같이 신청합니다.<div>'
        };        
        sendMail.sendMailByGmail(param);
        
        res.send(_result);
    });    
}

module.exports = {
    init_workInfo,
    init_workUsers,
    workinfo_add,
    workinfo_find,
    move_popupWorkInfo,
    workInfo_update,
    init_workVac,
    workInfo_delete,
    workInfo_updateAll
}