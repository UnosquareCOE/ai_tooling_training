import express, { json, urlencoded } from "express";
import cors from "cors";
import morgan from "morgan";

import { GamesRouter } from "./routers/games";

const app = express();

app.use(cors());
app.use(json());
app.use(morgan("tiny"));
app.use(urlencoded({ extended: true }));
app.use("/games", GamesRouter);

app.listen(4567, () => console.log("Application running on port 4567"));
