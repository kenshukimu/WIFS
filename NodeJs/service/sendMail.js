const nodemailer = require('nodemailer');
const config = require('../config/config');

const logger = require('../config/winston')('server');

var sendMail = {};

sendMail.sendMailByGmail = 
function sendMailByGmail (param) {
    let transporter = nodemailer.createTransport({
        // 사용하고자 하는 서비스, gmail계정으로 전송할 예정이기에 'gmail'
        service: 'gmail',
        // host를 gmail로 설정
        host: 'smtp.gmail.com',
        port: 587,
        secure: false,
        auth: {
          // Gmail 주소 입력, 'testmail@gmail.com'
          user: config.gmailID,
          // Gmail 패스워드 입력
          pass: config.gmailPassword,
        },
    });

    let mailOptions = {
        from: '대한상공회의소',        //보내는 사람 주소
        to: param.toMail,           //받는 사람 주소
        subject: param.subJect,         //제목
        html: param.html               //본문
    };

    transporter.sendMail(mailOptions, function(error, info){
        if (error) {
            //에러
            logger.error('Error sending email : ' + error);
        }
        //전송 완료
        //console.log("Finish sending email : " + info.response);        
        logger.info("Finish sending email : " + info.response);
        transporter.close()
    })
}

module.exports = sendMail;