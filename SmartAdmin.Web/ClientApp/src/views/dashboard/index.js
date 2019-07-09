import Loadable from "react-loadable";
import { Loading } from "../../common/navigation";

const Dashboard = Loadable({
  loader: () => import("./components/Dashboard"),
  loading: Loading
});

const Social = Loadable({
  loader: () => import("./components/Social"),
  loading: Loading
});

export const routes = [
  {
    path: ["/", "/dashboard/analytics"],
    exact: true,
    name: "Dashboard",
    component: Dashboard
  },
  {
    path: "/dashboard/social-wall",
    exact: true,
    name: "Social",
    component: Social
  }
];
