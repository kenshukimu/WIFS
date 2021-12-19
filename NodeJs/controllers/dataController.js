/*
* TITLE  : 데이터 출력 컨트롤
* AUTHOR : 김현수
* DATE   : 2021.01.25
* DESC   : 데이터 출력에 관련된 DAO서비스
*/

//const express = require('express')
const pdf = require('html-pdf');
const fs = require('fs');
const path = require('path');
const htmlmaker = require('../service/htmlMaker');
const makeExcel = require('../service/makeExcel');
const utils = require('../utils/utils');

const init_export = (req, res) => {
     //로그인 세션확인처리
     if(utils.isEmpty(req.session.userId)) {
        res.render('index' , {message:''});     
    }else{
        res.render('dataExport', {role:req.session.role, userId:req.session.userId, projectNm:req.session.dept});  
    }
}

//PDF작성
const prt_overTime_report = (req, res) => {
    
    let date_ob = new Date();
    let date = ("0" + date_ob.getDate()).slice(-2);
    let month = ("0" + (date_ob.getMonth() + 1)).slice(-2);
    let year = date_ob.getFullYear();
    let hours = date_ob.getHours();
    let minutes = date_ob.getMinutes();
    let seconds = date_ob.getSeconds();

    var _file_name = year
                     + "" + month 
                     + "" + date 
                     + "" + hours 
                     + "" + minutes 
                     + "" + seconds 
                     + "" + req.body._id + ".pdf";
    
    var _param = {
        name : req.body.name,
        workDate : req.body.workDate,
        workTimeS : req.body.workTimeS,
        workTimeE : req.body.workTimeE,
        workOver : req.body.workOver,
        overTimeReason : req.body.overTimeReason,
        dept : req.session.dept
    };

    var html = htmlmaker.htmlMaker_Approve(_param, req.body._kb);

    var option;

    if(req.body._kb == '1') {
        options = { format: 'A4',
        header: {
            "height": "45mm",
            "margin" : "10,0,0,0",
            "contents": '<div style="text-align:center;font-weight: bold; font-size: 2.0em;line-height: 1.0em;font-family: 돋움체;margin:60px;">야근 신청서</div>'
        },
        footer: {  
            "height":"50mm", 
            "contents": '<div style="text-align:center;font-weight: bold; font-size: 2.0em;line-height: 1.0em;font-family: 돋움체;">위와 같이 야근을 신청합니다.</div>'
        },
        type: "pdf", 
        quality: "75"
      };
    
    }else{
        options = { format: 'A4',
        header: {
            "height": "45mm",
            "contents": '<div style="text-align:center;font-weight: bold; font-size: 2.0em;line-height: 1.0em;font-family: 돋움체;margin:60px;">휴가 신청서</div>'
        },
        footer: {  
            "height":"50mm", 
            "contents": '<div style="text-align:center;font-weight: bold; font-size: 2.0em;line-height: 1.0em;font-family: 돋움체;">위와 같이 휴가을 신청합니다.</div>'
        },
        type: "pdf", 
        quality: "75"
      };
    
    }
   
    pdf.create(html, options).toFile(path.join('/home/ec2-user/WIFS/','/filetmp/') + _file_name, function(err, _res) {
        if (err) return console.log(err);       
        console.log(res);

        var param = {filename : _file_name};

        res.send(param);
    });
}

//PDF출력
const savePdf = (req, res) => {
    var _filePath = "/home/ec2-user/WIFS/filetmp/";
    res.setHeader('Content-type', 'application/pdf'); // 파일 형식 지정
    res.download(_filePath + req.query.fileName, req.query.NewFileName+ ".pdf" , function(err){
        if(err){
            res.json({err:err});
        }else{
            res.end();
        }
    });
}

//엑셀출력
const saveAsExcel = (req, res) => {
    var database = req.app.get('database');
    var _dataSet = new Object();
    var _data = new Array();
    var _headerList = [
                        ['id','아이디'],
                        ['name','이름'],
                        ['workDate','근무일자'],
                        ['workTimeS','근무시작시간'],
                        ['workTimeE','근무종료시간'],
                        ['workHour','총근무시간'],
                        ['workOver','연장근무시간'],
                        ['overTimeReason','연장근무사유']
                      ];

    var param = new Object();
    var sDate = req.body.startDate.replace(/-/gi, "");
    var eDate =  req.body.endDate.replace(/-/gi, "");

    //검색조건 설정이 안되었을 경우
    if(sDate == '') {
        sDate = '20000101';
    }
    if(eDate == '') {
        eDate = '20300101';
    }

    if(req.body.selUserList != 'init') {
        param.id = req.body.selUserList;
    }
    
    param.departNo = req.session.deptNo;
    
    database.UserModel.findUserWithWorkList(param, '1', sDate, eDate,function(err, result) {    
        result.forEach(obj => {           
            Object.entries(obj).forEach(([key, value]) => {
                var userName = obj.name;    
                if(`${key}`== 'workinfoList') {
                    if(obj.workinfoList.length > 0) {
                        
                        obj.workinfoList.forEach(element => {
                            var _item = new Object();
                            
                            _item.id = element.id;
                            _item.name = userName;
                            _item.workDate = element.workDate;
                            _item.workTimeS = element.workTimeS;
                            _item.workTimeE = element.workTimeE;
                            _item.workHour = element.workHour;
                            _item.workOver = element.workOver;
                            _item.overTimeReason = element.overTimeReason;

                            _data.push(_item);
                        });
                    }
                }
            }); 
        });       

        _dataSet.data = _data;
        _dataSet.excelTitle = makeExcel.makeExcelFileHeader(_headerList);

        param = new Object();
        param.dataSet = _dataSet;

        const report = makeExcel.makeExcelFile(param, '출근일람'); 
        res.send({content: report.toString('base64'), filename: 'testFile', result:true}); 
    });
}

module.exports = {
    init_export,
    prt_overTime_report,
    savePdf,
    saveAsExcel
}