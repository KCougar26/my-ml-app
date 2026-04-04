# main.py
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
import joblib
import pandas as pd

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"], 
    allow_methods=["*"],
    allow_headers=["*"],
)

model = joblib.load("fraud_model_v2"
".save")

@app.post("/predict")
async def predict_fraud(order_data: dict):
    df = pd.DataFrame([order_data])
    proba = model.predict_proba(df)[0, 1]
    
    return {
        "probability": round(float(proba), 4),
        "risk_level": "HIGH" if proba >= 0.7 else "MEDIUM" if proba >= 0.4 else "LOW",
        "is_fraud": bool(proba >= 0.4)
    }