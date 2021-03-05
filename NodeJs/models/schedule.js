var Schema = {};
Schema.createSchema = function(mongoose) {
    var ScheduleSchema = mongoose.Schema({
        id:{
            type:String,
            required:true
        },
		subject: {
            type:String,
            required:true,
            trim: true
        },
        body:{
            type:String,
            required:false,
            trim: true
            
        },
        location:{
            type:String,
            required:false,
            trim: true
        },
        properties:{
            type:String,
            required:false,
            trim: true
        },
        start:{
            type:String,
            required:false,
            trim: true
        },
        end:{
            type:String,
            required:false,
            trim: true
        }
    }, {timestamps:true});

    mongoose.model('Schedules', ScheduleSchema);
    console.log('Schedules 정의함.');
    
    // 스키마에 인스턴스 메소드 추가
	ScheduleSchema.methods = {
		saveSchedule: function(callback) {
            var self = this;
			this.validate(function(err) {
				if (err) return callback(err);
				
                self.save(callback);  
			});
        }
    }    
    
    //스키마에 STATICS 메소드 추가
    ScheduleSchema.statics = {
        removeSchedule:function(param, callback) {
            this.deleteMany(param, function(err,doc) {
                if(err) return callback(err);

                console.log(doc.deletedCount + '개의 객체를 삭제하였습니다..');

                callback(null, doc);
            });
        },
        findSchedule:function(param, callback) {                 
            this.find(param)
            .sort('id')
            .sort('start')
            .then((result) => {
                callback(null,result);
            })
            .catch((err) => {
                console.log(err);
            });
        },
        updateSchedule:function(param,updateParam,callback) {                   
            this.updateOne(param, {$set : updateParam})
            .then((result) => {
                callback(null,result);
            })
            .catch((err) => {
                console.log(err);
            });
        },
    }
	return ScheduleSchema;
};

module.exports = Schema;