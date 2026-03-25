from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List, Dict, Optional
import uuid

app = FastAPI()

poll_db: Dict[str, "Poll"] = {}

class PollCreate(BaseModel):
    name: str
    description: str
    options: List[str]

class PollUpdate(BaseModel):
    name: Optional[str] = None
    description: Optional[str] = None
    options: Optional[List[str]] = None

class Poll(BaseModel):
    id: str
    name: str
    description: str
    options: List[str]
    votes: Dict[str, int]

class VoteCast(BaseModel):
    option: str


@app.post("/polls", response_model=Poll, status_code=201)
async def create_poll(poll_in: PollCreate):
    poll_id = str(uuid.uuid4())
    
    initial_votes = {option: 0 for option in poll_in.options}
    
    poll = Poll(
        id=poll_id,
        name=poll_in.name,
        description=poll_in.description,
        options=poll_in.options,
        votes=initial_votes
    )
    
    poll_db[poll_id] = poll
    return poll


@app.get("/polls/{poll_id}", response_model=Poll)
async def get_poll(poll_id: str):
    if poll_id not in poll_db:
        raise HTTPException(status_code=404)
    
    return poll_db[poll_id]


@app.put("/polls/{poll_id}", response_model=Poll)
async def update_poll(poll_id: str, poll_update: PollUpdate):
    if poll_id not in poll_db:
        raise HTTPException(status_code=404)
    
    existing_poll = poll_db[poll_id]
    
    if poll_update.name is not None:
        existing_poll.name = poll_update.name
        
    if poll_update.description is not None:
        existing_poll.description = poll_update.description
        
    if poll_update.options is not None:
        existing_poll.options = poll_update.options
        
        new_votes = {}
        for opt in poll_update.options:
            new_votes[opt] = existing_poll.votes.get(opt, 0)
        existing_poll.votes = new_votes
        
    return existing_poll


@app.delete("/polls/{poll_id}", status_code=204)
async def delete_poll(poll_id: str):
    if poll_id not in poll_db:
        raise HTTPException(status_code=404)
    
    del poll_db[poll_id]
    return


@app.post("/polls/{poll_id}/vote")
async def cast_vote(poll_id: str, vote: VoteCast):
    if poll_id not in poll_db:
        raise HTTPException(status_code=404)
    
    poll = poll_db[poll_id]
    
    if vote.option not in poll.options:
        raise HTTPException(status_code=400)
        
    poll.votes[vote.option] += 1
    
    return {
        "message": "Vote cast successfully!",
        "current_results": poll.votes
    }