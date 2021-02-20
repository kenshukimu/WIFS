const express = require('express')
const pdf = require('html-pdf');
const fs = require('fs');
const path = require('path');
const htmlmaker = require('../service/htmlMaker');

const init_export = (req, res) => {
     //로그인 세션확인처리
     if(req.session.userId == '' || req.session.userId  === undefined) {
        res.render('index' , {message:''});     
    }else{
        res.render('dataExport', {role:req.session.role, userId:req.session.userId, projectNm:req.session.dept});  
    }
}

const prt_overTime_report = (req, res) => {

    console.dir(req.body);
    
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

    var html = htmlmaker.htmlMaker_Approve(_param);

    var option;

    if(req.body._kb == '1') {
        options = { format: 'A4',
        header: {
            "height": "45mm",
            "contents": '<div style="text-align:center;font-weight: bold; font-size: 2.0em;line-height: 1.0em;font-family: 돋움체;">야근 신청서</div>'
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
            "contents": '<div style="text-align:center;font-weight: bold; font-size: 2.0em;line-height: 1.0em;font-family: 돋움체;">휴가 신청서</div>'
        },
        footer: {  
            "height":"50mm", 
            "contents": '<div style="text-align:center;font-weight: bold; font-size: 2.0em;line-height: 1.0em;font-family: 돋움체;">위와 같이 휴가을 신청합니다.</div>'
        },
        type: "pdf", 
        quality: "75"
      };
    
    }
   
    console.log(html);

    pdf.create(html, options).toFile("./filetmp/" + _file_name, function(err, _res) {
        if (err) return console.log(err);       
        console.log(res);

        var param = {filename : _file_name};

        res.send(param);
    });
}

const savePdf = (req, res) => {
    var _filePath = "D:/private_folder/Kenshukimu'sPlayground/NodeJs/filetmp";
    res.setHeader('Content-type', 'application/pdf'); // 파일 형식 지정
    res.download(_filePath + "/" + req.query.fileName, req.query.NewFileName+ ".pdf" , function(err){
        if(err){
            res.json({err:err});
        }else{
            res.end();
        }
    });
}

module.exports = {
    init_export,
    prt_overTime_report,
    savePdf
}