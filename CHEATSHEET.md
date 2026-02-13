## Standard query definitions
https://www.mongodb.com/docs/manual/tutorial/query-documents/

db.collection.find(
  { status: "active" },           
  { name: 1, email: 1, _id: 0 }   
).sort({ createdAt: -1 })    

db.collection.find({ Contacts.0._id: { $regex: /^.{2,}$/ } })

Notes:
1 in projection = include field
0 in projection = exclude field
1 in sort = ascending
-1 in sort = descending

## Query by dates
db.collection.find({
  createdAt: {
    $gte: ISODate("2025-01-01T00:00:00Z"),
    $lte: ISODate("2025-12-31T23:59:59Z")
  }
})

## Explain: Get insights about the query
db.collection.find({ status: "active" }).explain()

## Standard update definitions
// Update a single field
db.collection.updateOne(
  { _id: ObjectId("507f1f77bcf86cd799439011") },
  { $set: { status: "active" } }
)

// Update multiple documents
db.collection.updateMany(
  { status: "pending" },
  { 
    $set: { status: "active", activatedAt: new Date() }
  }
)

db.collection.updateOne(
  { _id: ObjectId("507f1f77bcf86cd799439011") },
  { $inc: { views: 1 } }
)

Common update operators:
$set - Set field value
$inc - Increment numeric field
$unset - Remove field
$rename - Rename field
$push - Add item to array
$pull - Remove item from array
$addToSet - Add to array if not exists
$setOnInsert - Set value only on insert during upsert

## Standard delete definitions
// Delete by specific field
db.collection.deleteOne(
  { email: "user@example.com" }
)

// Delete all documents older than a specific date
db.collection.deleteMany(
  { 
    createdAt: { 
      $lt: ISODate("2025-01-01T00:00:00Z") 
    } 
  }
)

## Aggregation pipeline
db.sourceCollection.aggregate([
  { $match: {} },
  { $out: "targetCollection" } // It cleans output collection
]);

[
  {
    $match: {
      "Items.ItemType": "Trip"
    }
  },
  {
    $addFields: {
      totalAmountNumber: { $toDouble: "$Breakdown.BalanceDue" },
      tripCount: {
        $size: {
          $filter: {
            input: "$Items",
            as: "item",
            cond: { $eq: ["$$item.ItemType", "Trip"] }
          }
        }
      }
    }
  },
  {
    $group: {
      _id: null,
      totalCartsWithTrips: { $sum: 1 },
      avgTripsPerCart: { $avg: "$tripCount" },
      avgTotalAmount: { $avg: "$totalAmountNumber" },
      minTotalAmount: { $min: "$totalAmountNumber" },
      maxTotalAmount: { $max: "$totalAmountNumber" }
    }
  },
  {
    $project: {
      _id: 0,
      totalCartsWithTrips: 1,
      avgTripsPerCart: { $round: ["$avgTripsPerCart", 2] },
      avgTotalAmount: { $round: ["$avgTotalAmount", 2] },
      minTotalAmount: { $round: ["$minTotalAmount", 2] },
      maxTotalAmount: { $round: ["$maxTotalAmount", 2] }
    }
  }
]