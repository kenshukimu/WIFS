/*
* TITLE  : 유저 관련 컨트롤
* AUTHOR : 김현수
* DATE   : 2021.01.25
* DESC   : 유저에 관련된 DAO서비스
*/
const express = require('express');
const utils = require('../utils/utils');

const user_login = (req, res, next) => {
    
    var database = req.app.get('database');

    var paramId = req.body.id || req.query.id;

    database.UserModel.findUser({"id":paramId}, function(err, result) {    
        
        var password;
        var _role;
        var _dept;
        var _deptNo;
        if(result.length == 0) {
            var message = "아이디가 존재하지 않습니다.";
            res.render('index' , {message:message}); 
        }else{
            
            //데이터 취득할 경우
            result.forEach(function(doc,err) {
                password = doc.password;
                _role = doc.role;
                _dept = doc.depart;
                _deptNo = doc.departNo;
            });

            if(req.body.password != password) {
                var message = "비밀번호가 맞지 않습니다.";
                res.render('index' , {message:message}); 
            }else{      
                req.session.userId = paramId;    
                req.session.role =  _role;
                req.session.dept = _dept;
                req.session.deptNo = _deptNo;
                res.render('workInfo', {role:_role, userId:req.session.userId, projectNm:req.session.dept});  
            }
        }
    });
}

const user_logout = (req, res) => {
    //로그아웃 세션삭제처리
    req.session.destroy(function(err) {
        if(err) { 
            console.log(err);
        }            
        res.render('index' , {message:''});    
    }); 
}

//유저 검색
const user_find = (req, res) => {
    var database = req.app.get('database');

    var users = new database.UserModel({
        id:req.body.id,
        password:req.body.password
    });

    database.UserModel.findUser({"id":req.body.id}, function(err, result) {
        var param = {
            userList:result
        };
        
        res.send(param);
    });
}

//유저 등록처리
const user_add = (req, res) => {

    var database = req.app.get('database');

    var depart = req.body.depart.split("^");

    var users = new database.UserModel({
        id:req.body.id,
        password:req.body.password,
        name:req.body.name,
        depart:depart[0],
        departNo:depart[1],
        role:"0"
    });
    
    //우선 같은 ID를 가진 사용자가 있는지 확인처리
    database.UserModel.findUser({"id":req.body.id}, function(err, result) {    
    
        var param = {
            users:result
        };

        if(result.length == 0) {
            users.saveUser(function(err, result) {
                if (err) return res.send(err);

                var message = "등록 되었습니다.";
                res.render('userReg' , {message:message, flag:true}); 
            }); 
        }else{
            var message = "등록된 아이디가 있습니다.";
            res.render('userReg' , {message:message, flag:false}); 
        }
    });
}

//유저 삭제처리
const user_delete = (req, res) => {
    var database = req.app.get('database');      

    var paramId = req.body.id || req.query.id;

    database.UserModel.removeUser({"id":paramId}, function(err, result) {
        res.send(result);
    });
}

//유저 전체보기
const user_all = (req, res) => {
    var database = req.app.get('database');

    var param = new Object();

    param.departNo = req.session.deptNo;

    database.UserModel.findUser(param, function(err, result) {

        var param = {
            userList:result
        };
        res.send(param);
    });
}

const user_update = (req, res, next) => {

    var database = req.app.get('database');

    var _role = "0";
    if(req.body.role == '사용자') _role = "1";
    
    database.UserModel.updateUser({"id":req.body.id}, {"role":_role}, function(err, result) {
       next();
    });
}

const init_userReg = (req, res) => {   
    res.render('userReg' , {message:'', flag:false});   
}

const user_with_workList_forChart = (req,res) => {    

    var database = req.app.get('database');
    var param = new Object();

    if(!utils.isEmpty(req.body.id))  {
        param.id = req.body.id;
    }
    
    param.departNo = req.session.deptNo;

    var sDate = '20000101';
    var eDate = '20300101';

    database.UserModel.findUserWithWorkList(param, req.body.status, sDate, eDate, function(err, result) {

        //chartjs 데이터 만들기
        var labelsArray = new Array();
        var chartDataArray = new Array();

        var aJsonArray = new Array();
        var aJson = new Object();
        var aJsonb = new Object();
        aJson.result = true;
        aJson.message = null;
        
        var overTime = 0;

        result.forEach(obj => {
            overTime = 0;
            
            Object.entries(obj).forEach(([key, value]) => {            
                if(`${key}`== 'name') {
                    labelsArray.push(`${value}`);
                }

                if(`${key}`== 'workinfoList') {
                    if(obj.workinfoList.length > 0) {
                        obj.workinfoList.forEach(element => {
                            overTime += element.workOver;
                        });

                        chartDataArray.push(overTime);
                    }
                }
            });
        });      
        
        aJsonb.labels = labelsArray;
        aJsonb.chartData = chartDataArray;
        aJson.data = aJsonb;
        
        var sJson = JSON.stringify(aJson);
        res.send(sJson);
    });
}

const user_with_workList= (req,res) => {    

    var database = req.app.get('database');
    var param = new Object();

    if(!utils.isEmpty(req.body.id)) {
        param.id = req.body.id;
    }

    param.departNo = req.session.deptNo;

    var sDate = '20000101';
    var eDate = '20300101';

    database.UserModel.findUserWithWorkList(param, req.body.status, sDate, eDate, function(err, result) {

        var param = {
            approveList:result
        };

        res.send(param);
    });
}

module.exports = {
    user_login,
    user_logout,
    user_find,
    user_add,
    user_all,
    user_delete,
    user_update,
    init_userReg,
    user_with_workList_forChart,
    user_with_workList
}