import Division from './Division';
import UnitOfMeasure from './UnitOfMeasure';
import WeightClass from './WeightClass';

export default interface LeaderboardModel {
    name: string;
    uom: UnitOfMeasure;
    division: Division;
    weightClass: WeightClass;
}
