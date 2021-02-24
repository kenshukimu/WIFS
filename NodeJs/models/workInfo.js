var Schema = {};
Schema.createSchema = function(mongoose) {
    var WorkInfoSchema = mongoose.Schema({
        id:{
            type:String,
            required:true
        },
		workDate: {
            type:String,
            required:true,
            trim: true
        },
        workTimeS:{
            type:String,
            required:true,
            trim: true
            
        },
        workTimeE:{
            type:String,
            required:false,
            trim: true
        },
        workHour:{
            type:Number,
            required:false,
            trim: true
        },
        workOver:{
            type:Number,
            required:false,
            trim: true
        },
        dinnerTime:{
            type:Number,
            required:false,
            trim: true
        },
        lunchTime:{
            type:Number,
            required:false,
            trim: true
        },
        status:{
            type:String,
            required:true,
            trim: true
        },
        overTimeReason:{
            type:String,
            required:false
        }
    }, {timestamps:true});

    mongoose.model('WorkInfos', WorkInfoSchema);
    console.log('WorkInfos 정의함.');
    
    // 스키마에 인스턴스 메소드 추가
	WorkInfoSchema.methods = {
		saveWorkInfo: function(callback) {
            var self = this;
			this.validate(function(err) {
				if (err) return callback(err);
				
                self.save(callback);  
			});
        }
    }    
    
    //스키마에 STATICS 메소드 추가
    WorkInfoSchema.statics = {
        removeWorkInfo:function(param, callback) {
            this.deleteMany(param, function(err,doc) {
                if(err) return callback(err);

                console.log(doc.deletedCount + '개의 객체를 삭제하였습니다..');

                callback(null, doc);
            });
        },
        findWorkInfo:function(param, callback) {                 
            this.find(param)
            .sort('id')
            .sort('workDate')
            .then((result) => {
                callback(null,result);
            })
            .catch((err) => {
                console.log(err);
            });
        },
        updateWorkInfo:function(param,updateParam,callback) {                   
            this.updateOne(param, {$set : updateParam})
            .then((result) => {
                callback(null,result);
            })
            .catch((err) => {
                console.log(err);
            });
        },
    }
	return WorkInfoSchema;
};

module.exports = Schema;