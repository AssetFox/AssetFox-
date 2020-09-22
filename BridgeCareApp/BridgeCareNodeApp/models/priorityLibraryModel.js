const mongoose = require("mongoose");
mongoose.set('useFindAndModify', false);

const { Schema } = mongoose;

const prioritySchema = new Schema({
    priorityLevel: { type: Number },
    year: { type: Number },
    criteria: { type: String }
});

const priorityLibrarySchema = new Schema({
    name: { type: String },
    owner: {type: String},
    shared: {type: Boolean},
    description: { type: String },
    priorities: [prioritySchema]
});

module.exports = mongoose.model('PriorityLibrary', priorityLibrarySchema, 'priorityLibraries');
