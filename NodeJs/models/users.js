var Schema = {};
Schema.createSchema = function(mongoose) {
    var UserSchema = mongoose.Schema({       
		id: {
            type:String,
            required:true
        },
        password:{
            type:String,
            required:true
        },
        name:{
            type:String,
            required:true
        },
        depart:{
            type:String,
            required:true
        },
        departNo:{
            type:String,
            required:true
        },
        role:{
            type:String,
            required:false,
            default : '0'
        }
    }, {timestamps:true});

    mongoose.model('Users', UserSchema);
    console.log('UserSchema 정의함.');
    
    // 스키마에 인스턴스 메소드 추가
	UserSchema.methods = {
		saveUser: function(callback) {
            var self = this;
            			
			this.validate(function(err) {
                if (err) return callback(err);
				
				self.save(callback);
			});
        }
    }    

    //생성 후 체크 하는 방법도 있음.
    UserSchema.methods.verify = function(password) {
        return this.password === password
    }
    
    
    //스키마에 STATICS 메소드 추가
    UserSchema.statics = {
        removeUser:function(param, callback) {
            this.deleteMany(param, function(err,doc) {
                if(err) return callback(err);

                console.log(doc.deletedCount + '개의 객체를 삭제하였습니다..');

                callback(null, doc);
            });
        },
        findUser:function(param,callback) {            
            this.find(param)
            .then((result) => {
                callback(null,result);
            })
            .catch((err) => {
                console.log(err);
            });
        },
        updateUser:function(param,updateParam,callback) {                   
            this.updateOne(param, {$set : updateParam})
            .then((result) => {
                callback(null,result);
            })
            .catch((err) => {
                console.log(err);
            });
        },
        findUserWithWorkList:function(param,subparam,callback) {              

            this.aggregate(
                [
                    { $lookup:
                        {
                            /*
                            from: 'workinfos',
                            localField: 'id',
                            foreignField: 'id',
                            as: 'workinfoList'
                            */
                            from: 'workinfos',
                            as: 'workinfoList',
                            let: { id: '$id' },
                            pipeline: [{
                                $match: {
                                    $expr: {
                                                $and: [
                                                            { $eq: ['$id', '$$id'] },
                                                            { $eq: ['$status', subparam] },
                                                    ]
                                            }
                                        }
                            }]
                        }
                    }
                    ,{ $match:param}
                    ,{ $sort: 
                        {"workinfos.workDate": -1, 
                         "workinfos.workTimeS": -1
                        }
                     }
                ]
            ).then((result) => {
                callback(null,result);
            })
            .catch((err) => {
                console.log(err);
            });
        }
    }
	return UserSchema;
};

module.exports = Schema;