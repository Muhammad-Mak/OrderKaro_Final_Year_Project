from fastapi import FastAPI
import pandas as pd
from prophet import Prophet
import requests
from datetime import datetime, timedelta
import urllib3

# Disable SSL warnings for self-signed certs (dev only)
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

app = FastAPI()
BACKEND_SALES_URL = "https://smartcafebackend.azurewebsites.net/api/analytics/sales-history"

@app.get("/forecast")
def forecast(item_id: int, days: int = 7):
    try:
        response = requests.get(BACKEND_SALES_URL, verify=False)
        response.raise_for_status()
        data = response.json()
    except Exception as e:
        return {"error": f"Failed to fetch data: {str(e)}"}

    df = pd.DataFrame(data)
    df = df[df["menuItemId"] == item_id]
    if df.empty:
        return []

    df = df.rename(columns={"date": "ds", "quantity": "y"})
    df["ds"] = pd.to_datetime(df["ds"])

    # Force daily frequency and fill missing dates with 0
    df = df.set_index("ds").asfreq("D", fill_value=0).reset_index()

    # ✅ Extend data to today's date
    today = pd.to_datetime(datetime.utcnow().date())
    if df["ds"].max() < today:
        missing_dates = pd.date_range(df["ds"].max() + timedelta(days=1), today)
        filler = pd.DataFrame({"ds": missing_dates, "y": 0})
        df = pd.concat([df, filler])

    # Prophet forecast
    model = Prophet()
    model.fit(df)

    future = model.make_future_dataframe(periods=days)
    forecast = model.predict(future)

    result = forecast[["ds", "yhat"]]
    result = result[result["ds"] > today].head(days)

    result = forecast[["ds", "yhat"]]
    result = result[result["ds"] > today].head(days)

    return [
        {
            "date": r["ds"].strftime("%Y-%m-%d"),
            "quantity": max(0, int(r["yhat"]))  # Clamp to zero
        }
        for _, r in result.iterrows()
    ]
